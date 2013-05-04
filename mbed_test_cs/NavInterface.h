#pragma once

#include "enumStructuresGICS.h"

public ref class NavInterface
{
public:
	NavInterface(void);
	~NavInterface(void);
	static bool NavInterfaceFound(void);
	static NavInterface ^OpenNavInterface(System::String ^baseName);
	void SimInterface(bool useSimIF);
	bool SimInterface();
	void GetPosVel(PosVelGICS *posVel);
	void SendTrigger(unsigned char triggerMask);
	unsigned char SendingTrigger(void);
	double TriggerTime(void);
	void TriggerEpochs(int &gpsEpoch, int &imuEpoch);
	void ShutterLag(int lag);
	int ShutterLag(void);
	double CloseProgress(void);
	int ReadyStatus(void);
	int VisibleSats(void);
	int SolutionSats(void);
	virtual void Close(void);
	virtual void ReadMessages(void);
	virtual void WriteMessages(void);
	virtual void ParseMessages(void);
	virtual void InitConnection(void);
	virtual void LogInterfaceType(void);
protected:
	static NavInterface ^TestGICS_B3(System::String ^baseName);
	static NavInterface ^TestSerial(System::String ^comID, 
									System::String ^baseName,
									bool isMBed);
	static array<System::String ^> ^FindComPorts(void);
	System::IO::Ports::SerialPort ^OpenSerial(System::String ^comID, int baudRate);
	void LogData(System::String ^str);
	void LogWarning(System::String ^str);

	bool simInterface_;
	PosVelGICS *posVel_;
	double trigTime_;
	int trigGPSEpoch_;
	int trigIMUEpoch_;
	unsigned char triggerMask_;
	System::String ^baseName_;
	System::Threading::Mutex ^navIFMutex_;
	int replayLength_;
	int replayPos_;
	int navReadyStatus_;
	int shutterLag_;  // time in ms to add to shutter time
	System::IO::TextWriter ^twLog_;
};
