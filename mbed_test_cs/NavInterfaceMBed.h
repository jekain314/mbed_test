#pragma once

////////////////////////////////////////////////////////////////////////////////////////
// mbed interface for the Waldo FCS
// see www.mbed.org for the explanation of the mbed microcontroller board
// provides a serial I/O data stream for communications to/from the mbed
// this object manages the serial interfac e to the mbed 
// for sending commands and receiviung the serial data responses
// the actual IMU and GPS data is being stored on the mbed's connected Flash disk 
////////////////////////////////////////////////////////////////////////////////////////

public ref class NavInterfaceMBed
{
public:

enum class NAVMBED_CMDS  //definition of the various commands that can be sent to the mbed
{
	RECORD_DATA_ON=0,		//turn on the data recording
	RECORD_DATA_OFF=1,		//turn off the data recording
	POSVEL_MESSAGE=2,		//get a velocity message
	STATUS_MESSAGE=3,		//get a status message (what's in this??)
	STREAM_POS_ON=4,		//stream the pos message from the GPS receiver connected to the mbed
	STREAM_POS_OFF=5,		//turn off the pos streaming
	LOG_INFO_ON=6,			//turn on the logging (what is this???)
	LOG_INFO_OFF=7,			//turn off the logging
	FIRE_TRIGGER=8			//fire a hardware trigger to the canon camera
};

public:
	NavInterfaceMBed(void);  //constructor -- this is the one used for the form
	NavInterfaceMBed(System::String ^comID, System::String ^baseName);  //what is this for ???
	void Close(void);
	void ReadMessages(void);
	void WriteMessages(void);
	void WriteMessages(bool forceWrite);  //what is this ???
	void InitConnection(void);
	void LogInterfaceType(void);  //what is this ???
	void LogData(System::String ^str); //what is this for??
	void LogWarning(System::String ^str);
	static array<System::String ^> ^FindComPorts(void);  //what is this for?
	void SendCommandToMBed(NAVMBED_CMDS commandIndex);
	array<System::String ^> ^MessagesRead(void);

protected:
	void InitPort(System::String ^comStr, System::String ^baseName);  
	System::IO::Ports::SerialPort ^OpenSerial(System::String ^comID, int baudRate);
	void InitMBed(void);
	bool TestMBed(void);
	
	System::IO::Ports::SerialPort ^serialPort_;
	System::IO::TextWriter ^twNavDat_;
	System::String ^baseName_;
	System::Threading::Mutex ^navIFMutex_;
	int serialReadThreshold_;
	bool serialInit_;
	bool writeNavFiles_;
	System::Collections::Generic::Queue<System::String ^> ^serialBuffer_;
	System::Collections::Generic::Queue<System::String ^> ^readBuffer_;
	System::Collections::Generic::Queue<System::String ^> ^writeBuffer_;
	bool sentRecordOff_;
	System::IO::TextWriter ^twLog_;
};
