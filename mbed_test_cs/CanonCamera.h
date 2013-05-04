#pragma once

#include "EDSDKTypes.h"

delegate EdsError handleObjectDelegate( EdsObjectEvent evt,
									    EdsBaseRef object,
										EdsVoid * context);
delegate EdsError handlePropertyDelegate( EdsPropertyEvent evt,
											EdsPropertyID prop,
											EdsUInt32 parameter,
											EdsVoid * context);
delegate EdsError handleStateDelegate( EdsPropertyEvent evt,
											EdsPropertyID prop,
											EdsUInt32 parameter,
											EdsVoid * context);
ref class CanonCamera
{
public:
	CanonCamera(void);
	~CanonCamera(void);
	void FireTrigger(void);
	bool GetLiveFeedImage(int idx, System::Drawing::Bitmap ^%bmp);
	bool ImageReady();
	void StartLiveView(void);
	void EndLiveView(void);
	void SetPath(System::String ^path);
	System::String ^GetPath(void);
	void ListVolume(void);
	void SetShutters(double shutter);
	void GetISO(int &isoSpeed, int idx);
	void SetISOs(int isoSpeed);
	EdsObjectEventHandler objHandler;
	handleObjectDelegate ^objDelegate;
	EdsPropertyEventHandler propHandler;
	handlePropertyDelegate ^propDelegate;
	EdsPropertyEventHandler stateHandler;
	handleStateDelegate ^stateDelegate;
protected:
	void DownloadImage(EdsDirectoryItemRef directoryItem, EdsChar *path);
	void GetCameras();
	void ListVolumes(EdsCameraRef &camera);
	void ListVolume(EdsVolumeRef &volRef);
	void ListDirectory(EdsDirectoryItemRef &dirRef);
	EdsError handleObjectEvent( EdsObjectEvent evt,
						EdsBaseRef object,
						EdsVoid * context);
	EdsError handlePropertyEvent( EdsPropertyEvent evt,
									EdsPropertyID prop,
									EdsUInt32 parameter,
									EdsVoid * context);
	EdsError handleStateEvent( EdsPropertyEvent evt,
									EdsPropertyID prop,
									EdsUInt32 parameter,
									EdsVoid * context);
	void SetShutter(double shutter, int idx);
	EdsUInt32 LookupShutterCode(double shutter);

	void SetISO(int isoSpeed, int idx);
	EdsUInt32 LookupISOCode(int isoSpeed);
	int GetISOFromCode(EdsUInt32 isoCode);

	bool triggerFired_;
	bool imageReady_;
	bool liveFeed_;
	System::DateTime triggerTime_;
	System::String ^imgPath_;
	EdsCameraListRef eosCameras_;
	EdsError eosErr_;
	System::IO::TextWriter ^twLog;
};
