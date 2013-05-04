#include "NavInterfaceMBed.h"
#include <stdio.h>
#include <string.h>

using namespace System;
using namespace System::Collections::Generic;
using namespace System::Diagnostics;
using namespace System::IO;
using namespace System::IO::Ports;
using namespace System::Management;
using namespace System::Runtime::InteropServices;
using namespace System::Threading;


//////////////////////////////////////////////////////////////////////////
//these Unions are not used -- looks they are intended for byte swapping
typedef union 
{
	unsigned short us;
	unsigned char uc[2];
} uscType;

typedef union 
{
	__int32 i;
	unsigned char uc[4];
} iucType;
////////////////////////////////////////////////////////////////////////////

NavInterfaceMBed::NavInterfaceMBed(void)
{
	InitMBed();

	//get a list of all COM ports
	array<String ^> ^comStr = FindComPorts();

	LogData(" found " + comStr->Length + "serial ports" );
	for each (String^ cp in comStr)
	{
		LogData(" found comPort " + cp);
	}

	//for the Waldo FCS mechanization ... 
	//NOTE: we may have the three serial ports open from the GPS receiver (OEM615 uses three serial ports)

	if ((comStr == nullptr) ||
		(comStr->Length == 0))
	{
		LogData(" found no Com Ports ");
		throw gcnew Exception("Could not open MBed Interface");
	}

	String ^baseName = "C:\\temp\\phoebusMBed";

	//loop through all the serial ports until we find the one with the correct baud rate
	for (int c = 0; (c < comStr->Length) && (serialPort_ == nullptr); c++)
	{
		try
		{
			LogData(" try to open serial port " + comStr[c]);
			InitPort(comStr[c], baseName);  //thows an exception it wont open
		}
		catch(...)
		{
			continue;  //try another port from the list
		}
	}

	if (serialPort_ == nullptr)
	{
		throw gcnew Exception("Could not open NavProc Interface");
	}
}

NavInterfaceMBed::NavInterfaceMBed(String ^comID, String ^baseName)
{
	InitMBed();

	try
	{
		InitPort(comID, baseName);
	}
	catch(...)
	{
		throw;
	}
}

array<System::String ^> ^NavInterfaceMBed::FindComPorts(void)
{
	///////////////////////////////////////////////////////////////////////////////////////
	//this procedure returns a list of all the connected COM ports for the PC computer
	///////////////////////////////////////////////////////////////////////////////////////

	//string array to hold the com port names 
	array<System::String ^> ^comPorts = gcnew array<System::String ^>(0);

	/*  ManagementObjectSearcher:::  Retrieves a collection of management objects based on a specified query. 
	This class is one of the more commonly used entry points to retrieving management information. 
	For example, it can be used to enumerate all disk drives, network adapters, processes 
	and many more management objects on a system, or 
	to query for all network connections that are up, services that are paused, and so on.  */ 

	ManagementObjectSearcher ^mgObjs;  
	String ^queryStr;

	queryStr = "SELECT * FROM Win32_PNPEntity";  //selection string for the ManagementObjectSearcher
	mgObjs = gcnew ManagementObjectSearcher(queryStr);  //perform the ManagementObjectSearcher procedure to get the COM ports
	for each (ManagementObject ^obj in mgObjs->Get())
	{
		for each (PropertyData ^prop in obj->Properties)
		{
			if (prop->Name == "Name")
			{
				String ^valStr = prop->Value->ToString();
				if (valStr->Contains("Port") &&
							(valStr->Contains("COM")))  //look specifically for the COM ports
				{
					int baseIdx = valStr->IndexOf("COM");
					int rIdx = valStr->IndexOf(")", baseIdx);
					int len = rIdx - baseIdx;
					comPorts->Resize(comPorts, comPorts->Length+1);
					comPorts[comPorts->Length-1] = valStr->Substring(baseIdx, len);
				}
			}
		}
	}
	delete mgObjs;

	return comPorts;
}
void NavInterfaceMBed::InitMBed(void)
{
	sentRecordOff_ = false;
	serialPort_ = nullptr;
	serialReadThreshold_ = 1;
	serialBuffer_ = gcnew Queue<String ^>();
	readBuffer_ = gcnew Queue<String ^>();
	writeBuffer_ = gcnew Queue<String ^>();
	serialPort_ = nullptr;
	serialInit_ = false;
	baseName_ = "";

	navIFMutex_ = gcnew Mutex();

	try  //try to create a log file on the PC
	{
		twLog_ = File::CreateText("C:\\temp\\navMBed.log");
	}
	catch(...)  //catch the error is its not created
	{
		twLog_ = nullptr;
		LogData("could nnot open the navMBed.log file");
	}
}

void NavInterfaceMBed::InitPort(String ^comID, String ^baseName)
{
	//////////////////////////////////////////////////////////////////////////////////////////////
	// Attempt to open an mbed serial port
	// criteria is that we can successfully open the port at the specified baud rate
	// plus we send and receive 5 status messages to ensure we have the right port
	// for the Waldo_FCS, we will also have three additional ports related to the GPS receiver
	// ths looks like it was set up to try multiple baud rates
	//////////////////////////////////////////////////////////////////////////////////////////////

	LogData("Test port : " + comID);  //present to the log the progress

	for (int b = 8; (b <= 8) && (serialPort_ == nullptr); b++)  //just goes through this once??
	{
		int baudRate = 115200*b;
		LogData("Test baudRate : " + baudRate.ToString("0"));  //present to progress to the log
		serialPort_ = OpenSerial(comID, baudRate);  //test the opening of the serial port at the baudRate
		if (serialPort_ == nullptr)  //see if it opened properly ...
		{
			LogData("Open serial failed. COMID= "+ comID);
			return;
		}
		else
		{
			LogData("Test Nav Proc");
			if (TestMBed())  //test the mbed by sending and receiveing test messages -- see below for this procedure
			{
				LogData("Test MBed Passed");
			}
			else
			{
				LogData("Close serial port");
				serialPort_->Close();
				delete serialPort_;
				serialPort_= nullptr;
			}
		}
	}
	if (serialPort_ == nullptr)  //where is the catch for this  -- up one level in the call??
	{
		throw gcnew Exception("Could not open MBed Interface at " + comID);
	}
	baseName_ = baseName;
}

bool NavInterfaceMBed::TestMBed(void)
{
	/////////////////////////////////////////////////////////////////////////
	//test to make sure this is a valid mbed port
	//the mbed miscrocontroller must be running the correct firmware 
	//the test conists of sending/receiving valid/expected message responses
	// the message request is for the STATUS message
	/////////////////////////////////////////////////////////////////////////

	if (serialPort_ == nullptr)  //serial port should be opened
	{
		return false;
	}

	array<unsigned char> ^buf = gcnew array<unsigned char>(128);

	int navCount = 0;  //looks like it becomes a messdage counter for this entry

	//  mbed test handshake to assess whether the data link is operative
	for (int t = 0; (t < 60) && (navCount < 5); t++)   //we send at least 5 messages and expect to get at least 5 responses as a mbed serial test.
	{
		LogData("Write Status Request.");  //write a status request and get a response if the port is working

		SendCommandToMBed(NAVMBED_CMDS::STATUS_MESSAGE);  //send a command to get the status message
		WriteMessages(true);  //write the message to the serial port

		//note here that we dont wait any time to receive the response .... 
		Thread::Sleep(100);  //just releases the thread to do something else ??

		//Thread::Sleep(10);
		String ^serMsg;
		try   //try the serial read
		{
			serMsg = serialPort_->ReadLine();  //read a complete line from the serial port
		}
		catch(...)  //catch an error is one occurred
		{
			serMsg = nullptr;
		}
		if (serMsg != nullptr)  //log the message if we received a message
		{
			LogData("Bytes Read : " + serMsg->Length.ToString("0"));
			LogData(serMsg);
			if (serMsg->IndexOf("WMsg") == 0)   //this counts any message that begins with "WMsg"
			{
				navCount++;  //valid message counter
			}
		}
		else
		{
			LogData("Bytes Read : 0");
		}
	}
	LogData("Nav Count : " + navCount.ToString("0"));
	return (navCount >= 5);    // what is the >= 5 for ??? 
}

void NavInterfaceMBed::InitConnection(void)
{
	///////////////////////////////////////////////////////////////////////
	// this procedure doesnt do much
	//creates a log file
	//turns off the mbed record operations and flushes the read buffer
	///////////////////////////////////////////////////////////////////////

	navIFMutex_->WaitOne();  //what does this do??

	try  //not sure what this does ...
	{
		//create the log file using the base address
		twNavDat_ = File::CreateText(baseName_ + "_msg.log");
	}
	catch(...)
	{
		navIFMutex_->ReleaseMutex();
		throw;
	}
	writeNavFiles_ = true;  //what is this for???
	serialInit_ = true;

	navIFMutex_->ReleaseMutex();

	LogData("Reset Record data.");

	//send a message to the mbed to turn off the data recording
	SendCommandToMBed(NAVMBED_CMDS::RECORD_DATA_OFF);
	WriteMessages(true);

	// Add delay to allow system time to flush any open files
	Stopwatch ^swDelay = Stopwatch::StartNew();
	while (swDelay->ElapsedMilliseconds < 250)
	{
		Thread::Sleep(1);  //loop through here til the Stopwatch shows 250msecs elapsed 
	}
	ReadMessages();
}

SerialPort ^NavInterfaceMBed::OpenSerial(String ^comID, int baud)
{
	////////////////////////////////////////////////////////////////////////////
	//open a serial port with a baud rate and other initialization/parameters
	////////////////////////////////////////////////////////////////////////////

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

void NavInterfaceMBed::LogInterfaceType(void)
{
	//just print a message to the log file declaring this is an mbed interface
	LogData("MBed Interface.");
}

void NavInterfaceMBed::Close(void)
{
	////////////////////////////////////////////////////////
	//orderly shutdown the mbed serial interface
	////////////////////////////////////////////////////////

	SendCommandToMBed(NAVMBED_CMDS::RECORD_DATA_OFF);
	// Manually initiate these calls to ensure message is sent
	// and log confirmation
	WriteMessages();
	// Allow time for command to be sent
	Thread::Sleep(125);
	ReadMessages();
	Thread::Sleep(125);

	//  Blocks the current thread until the current WaitHandle receives a signal
	navIFMutex_->WaitOne();

	LogData("Close MBed Interface.");
	writeNavFiles_ = false;
	if (twNavDat_ != nullptr)
	{
		twNavDat_->Close();
		twNavDat_ = nullptr;
	}
	if (serialPort_ != nullptr)
	{
		delete serialPort_;
		serialPort_ = nullptr;
	}
	navIFMutex_->ReleaseMutex();
}

void NavInterfaceMBed::ReadMessages(void)
{
	//make sure the serial link has been successfully established 
	if ((serialPort_ == nullptr) || 
		(!serialPort_->IsOpen) ||
		(serialBuffer_ == nullptr) || 
		(serialInit_ == false))
	{
		return;  //if not successfully established -- return
	}

	int bytesAvailable = serialPort_->BytesToRead;  //bytes that can be read from the serial port
	if (bytesAvailable <= serialReadThreshold_)  //dont bother reading if bytesToRaed aee less than a threshold
	{
		return;
	}

	int bytesRead = 0;

	try  //try reading the serial bytes from the port 
	{
		//  Enqueue::  Adds an object to the end of the WorkflowQueue. 
		serialBuffer_->Enqueue(serialPort_->ReadLine());
	}
	catch(...)
	{
		LogWarning("Error reading serial data");
		return;
	}

	//????????????????????????????????????????
	// not sure what's happening here 
	//????????????????????????????????????????

	if (writeNavFiles_)
	{
		//matching ReleaseMutex is below ...
		navIFMutex_->WaitOne();  //does this just act like a Critical Section ??? 

		while (serialBuffer_->Count > 0)
		{
			try
			{
				//Dequeue::  Removes and returns the object at the beginning of the Queue(Of T). 
				String ^curStr = serialBuffer_->Dequeue();
				readBuffer_->Enqueue(curStr);
				twNavDat_->WriteLine(curStr);
				twNavDat_->Flush();
			}
			catch(Exception ^rExc)
			{
				LogData("Error copying serial data to buffer : " + rExc->Message);
			}
		}

		navIFMutex_->ReleaseMutex();
	}
	return;
}

void NavInterfaceMBed::WriteMessages(void)
{
	//not sure what this is doing ??? 
	WriteMessages(false);  //this calls the "forceWrite" vesion of this procedure below
}

void NavInterfaceMBed::WriteMessages(bool forceWrite)
{
	//first check to see if the serial port is opened properly
	if ((serialPort_ == nullptr) || 
		(!serialPort_->IsOpen) ||
		(writeBuffer_ == nullptr) || 
		((serialInit_ == false) && !forceWrite))
	{
		return;
	}

	navIFMutex_->WaitOne();

	try  //try to write the message
	{
		while (writeBuffer_->Count > 0)
		{
			serialPort_->WriteLine(writeBuffer_->Dequeue());
		}
	}
	catch(...)
	{
		LogData("Transmit error");
		navIFMutex_->ReleaseMutex();
		throw;
	}

	navIFMutex_->ReleaseMutex();
}

array<String ^>^ NavInterfaceMBed::MessagesRead(void)
{
	array <String ^> ^msgs = nullptr;
	if (readBuffer_->Count == 0)
	{
		return msgs;
	}

	navIFMutex_->WaitOne();

	try
	{
		int msgCount = 0;
		msgs = gcnew array<String ^>(readBuffer_->Count);
		while (readBuffer_->Count > 0)
		{
			msgs[msgCount++] = readBuffer_->Dequeue();
		}

		navIFMutex_->ReleaseMutex();
	}
	catch(Exception ^exc)
	{
		LogData("Error copying serial data to parse buffer : " + exc->Message);

		navIFMutex_->ReleaseMutex();
	}
	return msgs;
}

void NavInterfaceMBed::SendCommandToMBed(NAVMBED_CMDS commandIndex)
{
	navIFMutex_->WaitOne();

	String ^msgStr;
	// Shared Message flag header
	msgStr = "WMsg ";
	// Message specific body
	switch (commandIndex)
	{
		case NAVMBED_CMDS::RECORD_DATA_ON:
		case NAVMBED_CMDS::RECORD_DATA_OFF:
			{
				msgStr += "RECORDDATA ";
				if (commandIndex == NAVMBED_CMDS::RECORD_DATA_ON)					
				{
					msgStr += "Y";
				}
				else
				{
					msgStr += "N";
				}
			}
			break;
		case NAVMBED_CMDS::STREAM_POS_ON:
		case NAVMBED_CMDS::STREAM_POS_OFF:
			{
				msgStr += "POSSTREAM ";
				if (commandIndex == NAVMBED_CMDS::STREAM_POS_ON)
				{
					msgStr += "Y";
				}
				else
				{
					msgStr += "N";
				}
			}
			break;
		case NAVMBED_CMDS::LOG_INFO_ON:
		case NAVMBED_CMDS::LOG_INFO_OFF:
			{
				msgStr += "LOGINFO ";
				if (commandIndex == NAVMBED_CMDS::LOG_INFO_ON)
				{
					msgStr += "Y";
				}
				else
				{
					msgStr += "N";
				}
			}
			break;
		case NAVMBED_CMDS::POSVEL_MESSAGE:
			{
				msgStr += "POSVEL";
			}
			break;
		case NAVMBED_CMDS::STATUS_MESSAGE:
			{
				msgStr += "STATUS";
			}
			break; 
		case NAVMBED_CMDS::FIRE_TRIGGER:
			{
				msgStr += "TRIGGER";
			}
		break;
	}
	// shared new line character
	writeBuffer_->Enqueue(msgStr);
	navIFMutex_->ReleaseMutex();
}

void NavInterfaceMBed::LogData(String ^str)
{
	////////////////////////////////////////////////
	//write a line to the log file
	////////////////////////////////////////////////
	if (twLog_ != nullptr)
	{
		twLog_->WriteLine(str);
		twLog_->Flush();
	}
}

void NavInterfaceMBed::LogWarning(String ^str)
{
	// log a warning -- not implemented ...
	;
}

//how to read the bytes stored on the mbed
/*
1) add a message to send to the mbed to initiate the transfer
2) add a button to initiate the transfer after a record session
3) make sure the record was previously started and stopped -- if not send info to user
4) The port is already opened so dont need to do this
5) The ongoing read process for message responses is controlled by the timer.
6) shut down the timer for the read bytes procedure
7) after the message has been sent to the mbed, initiate a continuous read loop to grab the bytes
8) check for a header to defines the number of bytes that will be sent
9) continuing reading the bytes 1-at-1-time and check for the closing byte stream from the mbed
10) store the bytes to the hard disk 
*/
