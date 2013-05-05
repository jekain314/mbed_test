using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using CanonCameraEDSDK;

namespace mbed_test_cs
{
    public partial class Form1 : Form
    {
        //This is the mbed device interface
        //should be instantiated upon the form load -- if no mbed found report back 
        NavInterfaceMBed navIF_;

        //this is the canon camera device interface (class library to the Canon SDK)
        //instantiate at the start -- if no camera found report back
        CanonCamera camera;
        String imageFilenameWithPath;
        bool threadWorking = false;
        bool triggerRequested = false;
        int POSVELmessagesReceivedThisSec = 0;
        int secCounter = 0;
        int sentTriggerCommand = 0;
        int receivedMbedResponseToTrigger = 0;
        int receivedImageOnCameraSDcard = 0;


        // This delegate is used for writing to the RTB control from within a thread 
        delegate void AppendRichTextDelegate(RichTextBox ctl, String str);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 3;
            comboBox2.SelectedIndex = 1;

            ////////////////////////////////////////////////////////////////////////////////////////////////
            //connect to the embed serial interface -- should be connected to the USB port on the PC
            //all the constructor does is to look for the serial port and
            //uses the one that has the correct behavior: receives a status message when requested
            //It is important that an mbed used with this interface presents a proper response to a message request 
            ////////////////////////////////////////////////////////////////////////////////////////////////

            try  //call the Nav interface constructor
            {
                Log(" call NavInterfaceMbed constructor");
                navIF_ = new NavInterfaceMBed();  //managed object constructor

                //start the background worker thread where the mbed messages are sent and camera info is processed
                this.backgroundWorker1.RunWorkerAsync();
                threadWorking = true;
                triggerRequested = false;

                //the serial read/write action is done in a timer -- start the timer when we have connected 
                this.timer1.Interval = 5000;
                timer1.Start();
            }
            catch  //catch the error if the initialization has failed
            {
                navIF_ = null;
            }
            if (navIF_ == null)
            {
                Log("Error connecting to mBed");
            }

            try
            {
                //all camera actions are performed in the cameraname
                camera = new CanonCamera();
            }
            catch
            {
                camera = null;
                Log("Error connecting to camera");
            }
        }

        private void btnConnectMbed_Click(object sender, EventArgs e)
        {
            

		 }

         //this procedure just prints to the rich text box display to the user ... 
		 void Log(String msg)
		 {
			 AppendRichText(richTextBox1, msg + "\r\n");
		 }


		 void AppendRichText(System.Windows.Forms.RichTextBox ctl, String str)
		 {
			if (ctl.InvokeRequired)
			{
                this.Invoke(new Action<string>(ctl.AppendText), new object[] { str });
			}
			else
			{
				ctl.AppendText(str);
				ctl.ScrollToCaret();  //Scrolls the contents of the control to the current caret position
				ctl.Refresh();
			}
		}

         private void button1_Click(object sender, EventArgs e)
         {
             
			 //////////////////////////////////////////////////////////////////////////////////////////////////////////////
			 //this sends a command across the Nav Interface serial interface to the mbed
			 //the messages are selected from a comboBox
			 // see the enum NAVMBED_CMDS (in the NavInterfaceMbed class) where the potential messages are declared
			 // for each message declared there, there must be a case selecrion provided below
			 //////////////////////////////////////////////////////////////////////////////////////////////////////////////

			 if (navIF_ == null)  //make sure the interface exists
			 {
				 Log("mBed not Connected.");  //print message if not connected
			 }

			 switch (this.comboBox2.SelectedIndex)  //switch based on the message selected from the pull down
			 {
				 case 0:
					 navIF_.SendCommandToMBed(NavInterfaceMBed.NAVMBED_CMDS.STATUS_MESSAGE);
				 break;
				 case 1:
					 navIF_.SendCommandToMBed(NavInterfaceMBed.NAVMBED_CMDS.POSVEL_MESSAGE);
				 break;
				 case 2:
					 navIF_.SendCommandToMBed(NavInterfaceMBed.NAVMBED_CMDS.RECORD_DATA_ON);
					 Thread.Sleep(100);
				 break;
				 case 3:
					 navIF_.SendCommandToMBed(NavInterfaceMBed.NAVMBED_CMDS.RECORD_DATA_OFF);
					 Thread.Sleep(100);
				 break;
				 case 4:
					 navIF_.SendCommandToMBed(NavInterfaceMBed.NAVMBED_CMDS.STREAM_POS_ON);
				 break;
				 case 5:
					 navIF_.SendCommandToMBed(NavInterfaceMBed.NAVMBED_CMDS.STREAM_POS_OFF);
				 break;
				 case 6:
					 navIF_.SendCommandToMBed(NavInterfaceMBed.NAVMBED_CMDS.LOG_INFO_ON);
				 break;
				 case 7:
					 navIF_.SendCommandToMBed(NavInterfaceMBed.NAVMBED_CMDS.LOG_INFO_OFF);
                     break;
				 case 8: 
					 navIF_.SendCommandToMBed(NavInterfaceMBed.NAVMBED_CMDS.FIRE_TRIGGER);
				 break;
			 }
         }

         private void timer1_Tick(object sender, EventArgs e)
         {
             triggerRequested = true;
         }

         private void btnInitCamera_Click(object sender, EventArgs e)
         {
         
 
            
         }

         private void btnSWTrigger_Click(object sender, EventArgs e)
         {
             if (camera != null)
             {
                 //fire a software trigger to the Canon camera
                 //The mbed will trap the event time of the actual image event using the hotshoe
                 camera.FireTrigger();
             }
         }

         private void btnSetShutter_Click(object sender, EventArgs e)
         {
			 //set the shutter for the Canon camera
			 try  //try to set the shutter
			 {
				 double shutterVal = Convert.ToDouble(this.textBox1.Text);
				 camera.SetShutters(shutterVal);
			 }
			 catch  //catch an error if thrown ad print a message to the RTB
			 {
				 richTextBox1.AppendText("Error setting shutter value.");
			 }
		 }

         private void btnSetISO_Click(object sender, EventArgs e)
         {
			 //set the camera ISO valoe
			 try  //try to set the ISO
			 {
				 int isoVal = Convert.ToInt32(this.comboBox1.SelectedItem.ToString());
				 camera.SetISOs(isoVal);
			 }
			 catch  //castch the error if one is thrown
			 {
				 richTextBox1.AppendText("Error setting iso value.");
			 }

		 }

         private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
         {
             //////////////////////////////////////////////////////////////////////
             //highspeed background thread for 
             //     (1 requesting/retrieving messages from mbed
             //     (2 processing images taken by the camer
             /////////////////////////////////////////////////////////////////////

             //to do:
             //  request and retrieve a POSVEL message at a high rate (e.g. 20 Hz);
             //  request a hardware trigger when requested by the foreground
             //  process any error messages received by the mbed and report to foreground
             //  detect missed responses from mbed and report to foreground
             //  check for the availability of a new image from the camera on its SD card
             //  dont allow a new trigger until a trigger request has had a response

             int loopCounter = 0;
             Stopwatch timeFromTrigger = new Stopwatch();
             timeFromTrigger.Start();
             bool nextTriggerAllowed = true;  //wait til the image has been saved to the camera's SD card before allowing next trigger
             timerPPS.Start();

             while (threadWorking)
             {
                 //count cycles through this loop (~ 100 Hz)
                 loopCounter++;

                 //triggerRequested set in foreground
                 if (triggerRequested)
                 {
                     AppendRichText(richTextBox1, "command mbed to fire a trigger" + "\r\n");
                     navIF_.SendCommandToMBed(NavInterfaceMBed.NAVMBED_CMDS.FIRE_TRIGGER);
                     navIF_.WriteMessages(); //if we have messages to write (commands to the mbed) then write them  

                     timeFromTrigger.Reset(); timeFromTrigger.Start();
                     triggerRequested = false;
                     sentTriggerCommand++;
                 }

                 //20Hz activities
                 if ( (loopCounter%2 == 0) && !triggerRequested)  //used to do things at a slower rate than loop cycles
                 {
                     POSVELmessagesReceivedThisSec++;
                     navIF_.SendCommandToMBed(NavInterfaceMBed.NAVMBED_CMDS.POSVEL_MESSAGE);
                     navIF_.WriteMessages(); //if we have messages to write (commands to the mbed) then write them  

                     loopCounter = 0;
                 }

                 //mbed message processing
                 if (navIF_ != null)  //test for the navIF presence (see the NavInterfaceMbed class specification) 
                 {
                     navIF_.ReadMessages();  //read any nav messages from the mbed -- there may be multiple messages
                     navIF_.ParseMessages();
                     //navIF_.WriteMessages();
                     /*
                     List<String> msgList = navIF_.MessagesRead();  //get the Possibly multiple) messages presented by the mbed serial interface

                     if (msgList != null)  //if there are messages present, present them to the rich text box to the user
                     {
                         foreach (String msg in msgList)  //may be multiple messages presented from the mbed
                         {
                             //test the message for an action:
                             //(1)  if TRIGGER  --- write the trigger file (assume for now we will bulb-trigger)
                             //(2)  if POSVEL   --- make it availble
                             //(3)  if ERROR    --- terminate and print the error

                             //anything that is written to the USB on the mbed using "printf" will be displayed on the RTB
                             //Log(msg);  //this writes messages to the Rich Text Box ...
                         }
                     }
                      * */

                     //we also write any queued messages to the serial port
                     //  (1) request POSVAL
                     //  (2) if time-to-trigger -- request trigger from mbed
                 }

                 if (camera != null)
                 {
                     if (camera.ImageReady(out imageFilenameWithPath))
                     {
                         //imageReady should come after a mbed trigger request
                         //set a delay and dont wait too long for another trigger
                         //get stats for the delay between the request and the imageReady
                         camera.resetImageReady();
                         AppendRichText(richTextBox1," camera Image:  " + imageFilenameWithPath  + "   dt=  " +  timeFromTrigger.ElapsedMilliseconds.ToString() +  "\r\n");
                         receivedImageOnCameraSDcard++;
                     }
                 }

                 if (navIF_.triggerTimeReceievdFromMbed)
                 {
                     AppendRichText(richTextBox1, "triggerTimeReceived:  " + navIF_.triggerTime.ToString() +  "\r\n");
                     navIF_.triggerTimeReceievdFromMbed = false;
                     receivedMbedResponseToTrigger++;
                 }


                 //we will nominally go through this loop at 100Hz (10 millisecs period)
                 Thread.Sleep(50);

             }  //end of while
         }

        private void timerPPS_Tick(object sender, EventArgs e)
         {
             
            AppendRichText(richTextBox1, secCounter.ToString() + "  POSVEL messages: " +
                 navIF_.numPosVelMsgs.ToString() + "/" + POSVELmessagesReceivedThisSec.ToString() + 
                 "  numSV=  " + navIF_.posVel_.numSV.ToString() + 
                 "  " + sentTriggerCommand.ToString() + 
                 "  " + receivedMbedResponseToTrigger.ToString() + 
                 "  " + receivedImageOnCameraSDcard.ToString() +
                 "\r\n" );
             POSVELmessagesReceivedThisSec = 0;
             navIF_.numPosVelMsgs = 0;
             secCounter++;

         }

        private void button2_Click(object sender, EventArgs e)
        {
            //stop the background worker thread
            threadWorking = false;

            //stop the timers
            timerPPS.Stop();
            timer1.Stop();

            //copy the IMU and GPS binary file to the PC
            navIF_.copyNavFileToPC();
        }  


     }  //end of Form1 Class

}  //end of the namespace
