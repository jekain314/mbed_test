#include "NavInterface.h"
#include "NavInterfaceGICS2.h"
#include "NavInterfaceGICS_B3.h"
#include "NavInterfaceNavProc.h"
#include "NavInterfaceMBed.h"
#include "NavInterfaceNIF.h"

using namespace System;
using namespace System::Diagnostics;
using namespace System::IO;
using namespace System::IO::Ports;
using namespace System::Management;
using namespace System::Threading;
using namespace System::Collections::Generic;

NavInterface::NavInterface(void)
{
	triggerMask_ = 0x00;
	replayLength_ = -1;
	replayPos_ = -1;
	navReadyStatus_ = 10;
	shutterLag_ = 0;
	twLog_ = nullptr;
	trigTime_ = -1.0;
	trigGPSEpoch_ = -1;
	trigIMUEpoch_ = -1;

}

NavInterface::~NavInterface(void)
{
	/*
	if (twLog_ != nullptr)
	{
		twLog_->Close();
		delete twLog_;
		twLog_ = nullptr;
	}
	*/
	Close();
	if (twLog_ != nullptr)
	{
		twLog_->Close();
		delete twLog_;
		twLog_ = nullptr;
	}
}

bool NavInterface::NavInterfaceFound(void)
{
	NavInterface ^navIF = nullptr;
	bool navFound = false;
	try
	{
		String ^baseName = nullptr;
		navIF = OpenNavInterface(nullptr);
	}
	catch(...)
	{
		if (navIF != nullptr)
		{
			delete navIF;
			navIF = nullptr;
		}
	}
	if (navIF != nullptr)
	{
		delete navIF;
		navIF = nullptr;
		navFound = true;
	}

	Thread::Sleep(125);
	return navFound;
}

NavInterface ^NavInterface::OpenNavInterface(String ^baseName)
{
	ManagementObjectSearcher ^mgObjs;
	String ^queryStr;
	NavInterface ^navIF = nullptr;
	List<String^> ^comPorts = gcnew List<String^>();

	bool mBedFound = false;
	Stopwatch ^swDelay = Stopwatch::StartNew();
	queryStr = "SELECT * FROM Win32_PNPEntity";
	mgObjs = gcnew ManagementObjectSearcher(queryStr);
	for each (ManagementObject ^obj in mgObjs->Get())
	{
		if (obj != nullptr)
		{
			for each (PropertyData ^prop in obj->Properties)
			{
				if (prop != nullptr)
				{
					if (prop->Name == "Name")
					{
						if (prop->Value != nullptr)
						{
							String ^valStr = prop->Value->ToString();
							// Check for Blackfin
							if (valStr->Contains("Blackfin"))
							{
								try
								{
									navIF = gcnew NavInterfaceGICS2(baseName);
								}
								catch(...)
								{
									if (navIF != nullptr)
									{
										delete navIF;
										navIF = nullptr;
									}
									}
								if (navIF != nullptr)
								{
									return navIF;
								}
								break;
							}
							else if (valStr->Contains("mbed") &&
									 valStr->Contains("COM"))
							{
								mBedFound = true;
								int baseIdx = valStr->IndexOf("COM");
								int rIdx = valStr->IndexOf(")", baseIdx);
								int len = rIdx - baseIdx;
								String ^comStr = valStr->Substring(baseIdx, len);
								navIF = TestSerial(comStr, baseName, true);
								if (navIF != nullptr)
								{
									return navIF;
									break;
								}
							}
							else if (valStr ->Contains("Port") &&
									 valStr ->Contains("COM"))
							{
								comPorts->Add(valStr);
							}
						}
					}
				}
			}
		}
	}
	if (!mBedFound)
	{
		navIF = TestGICS_B3(baseName);
		if (navIF == nullptr)
		{				
			for each (String ^comID in comPorts)
			{
				// Data stream on command is same as 
				// NavProc Double Trigger
				// allow time to clear camera
				while (swDelay->Elapsed.TotalSeconds < 2.0)
				{
					Thread::Sleep(3);
				}
				int baseIdx = comID->IndexOf("COM");
				int rIdx = comID->IndexOf(")", baseIdx);
				int len = rIdx - baseIdx;
				String ^comStr = comID->Substring(baseIdx, len);
				navIF = TestSerial(comStr, baseName, false);
				if (navIF != nullptr)
				{
					return navIF;
					break;
				}
			}
		}
	}
	delete mgObjs;
	return navIF;
}

void NavInterface::SimInterface(bool useSimIF)
{
	simInterface_ = useSimIF;
}

bool NavInterface::SimInterface()
{
	return simInterface_;

}

array<System::String ^> ^NavInterface::FindComPorts(void)
{
	array<System::String ^> ^comPorts = gcnew array<System::String ^>(0);
	ManagementObjectSearcher ^mgObjs;
	String ^queryStr;

	queryStr = "SELECT * FROM Win32_PNPEntity";
	mgObjs = gcnew ManagementObjectSearcher(queryStr);
	for each (ManagementObject ^obj in mgObjs->Get())
	{
		for each (PropertyData ^prop in obj->Properties)
		{
			if (prop->Name == "Name")
			{
				String ^valStr = prop->Value->ToString();
				if (valStr->Contains("Port") &&
							(valStr->Contains("COM")))
				{
					int baseIdx = valStr->IndexOf("COM");
					int rIdx = valStr->IndexOf(")", baseIdx);
					int len = baseIdx - rIdx;
					comPorts->Resize(comPorts, comPorts->Length+1);
					comPorts[comPorts->Length-1] = valStr->Substring(baseIdx, len);
				}
			}
		}
	}
	delete mgObjs;

	return comPorts;
}
NavInterface ^NavInterface::TestSerial(String ^comID, String ^baseName, bool isMBed)
{
	NavInterface ^navIF = nullptr;
	if (isMBed)
	{
		navIF = gcnew NavInterfaceMBed(comID, baseName);
	}
	else
	{
		try
		{
			navIF = gcnew NavInterfaceNavProc(comID, baseName);
			if (navIF != nullptr)
			{
				navIF->LogData("Found NavProc.");
			}
		}
		catch(...)
		{
			if (navIF != nullptr)
			{
				delete navIF;
				navIF = nullptr;
			}
		}
		if (navIF == nullptr)
		{
			try
			{
				navIF = gcnew NavInterfaceNIF(comID, baseName);
			}
			catch(...)
			{
				if (navIF != nullptr)
				{
					delete navIF;
					navIF = nullptr;
				}
			}
		}
	}
	return navIF;
}
NavInterface ^NavInterface::TestGICS_B3(String ^baseName)
{
	NavInterface ^navIF = nullptr;
	try
	{
		navIF = gcnew NavInterfaceGICS_B3(baseName);
		if (navIF != nullptr)
		{
			navIF->LogData("Found Nav GICS B3.");
		}
	}
	catch(...)
	{
		if (navIF != nullptr)
		{
			delete navIF;
			navIF = nullptr;
		}
	}
	return navIF;
}

SerialPort ^NavInterface::OpenSerial(String ^comID, int baud)
{
	SerialPort ^sPort;
	sPort = gcnew SerialPort(comID);
	sPort->Open();
	sPort->DataBits = 8;
	sPort->Parity = Parity::None;
	sPort->StopBits = StopBits::One;
	sPort->RtsEnable = false;
	sPort->DtrEnable = false;
	sPort->ReadTimeout = 1;
	sPort->BaudRate = baud;
	return sPort;
}

void NavInterface::GetPosVel(PosVelGICS *posVel)
{
	*posVel = *posVel_;
}

void NavInterface::SendTrigger(unsigned char triggerMask)
{
	triggerMask_ |= triggerMask;
}

unsigned char NavInterface::SendingTrigger(void)
{
	return triggerMask_;
}

double NavInterface::TriggerTime(void)
{
	return trigTime_;
}

void NavInterface::TriggerEpochs(int &gpsEpoch, int &imuEpoch)
{
	gpsEpoch = trigGPSEpoch_;
	imuEpoch = trigIMUEpoch_;
}

void NavInterface::ShutterLag(int lag)
{
	shutterLag_ = lag;
}

int NavInterface::ShutterLag(void)
{
	return shutterLag_;
}

double NavInterface::CloseProgress(void)
{
	double prog = -1.0;
	if (replayLength_ > 0)
	{
		prog = (double)replayPos_/(double)replayLength_;
	}
	return prog;
}

int NavInterface::ReadyStatus(void)
{
	return navReadyStatus_;
}
int NavInterface::VisibleSats(void)
{
	return posVel_->numSV;
}
int NavInterface::SolutionSats(void)
{
	return posVel_->solSV;
}

void NavInterface::Close(void)
{
	;
}

void NavInterface::ReadMessages(void)
{
	;
}

void NavInterface::WriteMessages(void)
{
	triggerMask_ = 0x00;
}

void NavInterface::ParseMessages(void)
{
	;
}

void NavInterface::InitConnection(void)
{
	;
}
