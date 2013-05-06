using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Ports;
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
 

        int POSVELmessagesReceivedThisSec = 0;
        int secCounter = 0;
        int posVelTicks = 0;

        bool waitingForPOSVEL = true;
        bool waitingForTriggerResponse = true;

        Stopwatch timeFromTrigger = new Stopwatch();
        long elapsedTimeToTrigger = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 3;

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

            this.timerPosVel.Interval = 100;
            timerPosVel.Start();
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

        private void timerPPS_Tick(object sender, EventArgs e)
         {
             
            AppendRichText(richTextBox1, secCounter.ToString() + "  POSVEL messages: " +
                 navIF_.numPosVelMsgs.ToString() + "/" + POSVELmessagesReceivedThisSec.ToString() + 
                 "  numSV=  " + navIF_.posVel_.numSV.ToString() + 
                 "\r\n" );
             POSVELmessagesReceivedThisSec = 0;
             navIF_.numPosVelMsgs = 0;
             secCounter++;

         }

        private void button2_Click(object sender, EventArgs e)
        {
            //stop the requests for triggers and PosVel
            timerPosVel.Stop();
        }

        //this thread is started in the timerPosVel_Tick()
        //it requests a PosVel and waits for a response.
        //the timerPosVel_Tick() also waits and is released whrn the thread completes
        private void PosVelThread_DoWork(object sender, DoWorkEventArgs e)
        {
            bool gettingRequestedPosvel = true;

            navIF_.SendCommandToMBed(NavInterfaceMBed.NAVMBED_CMDS.POSVEL_MESSAGE);
            navIF_.WriteMessages(); //if we have messages to write (commands to the mbed) then write them  

            while (gettingRequestedPosvel)
            {
                //read the data received from the mbed to check for a PosVel message
                navIF_.ReadMessages(); 
                navIF_.ParseMessages();

                //if received, exit this loop and the receive message event will be fired
                if (navIF_.PosVelMessageReceived) gettingRequestedPosvel = false;
            }

            waitingForPOSVEL = false;
        }

        private void timerPosVel_Tick(object sender, EventArgs e)
        {
            ////////////////////////////////////////////////////////////////////////////////
            //this timer will go off and get the PosVel data and wait til it is received
            // With the Waldo_FCS, there is no need to update the platform/flightLine geometry unless we get a new PosVel




            //start the thread to go get the PosVel from the mbed
            //this send the PosVel command to mbed and stars a loop to read the mbed data
            //when the data is received, the thread exits, and the threadCompleted event is fired
            //this completion event merely sets waitingForPOSVEL to false

            if ((posVelTicks % 50) != 0)
            {
                waitingForPOSVEL = true;
                this.PosVelThread.RunWorkerAsync();
                while (waitingForPOSVEL) { }
            }


            //here we will compute the platform geometry and test for a trigger requirement
            //for mbed_test, we will simultate this by just waiting for an elepsed number of ticks
            //and then do a trigger request
            //note that the PosVel thread and the triggerRequest threads never are running sequentially

            if ((posVelTicks % 50) == 0)
            {
                this.CameraImageReturnedThread.RunWorkerAsync();

                AppendRichText(richTextBox1, "command mbed to fire a trigger" + "\r\n");
                waitingForTriggerResponse = true;
                this.triggerRequestThread.RunWorkerAsync();
                while (waitingForTriggerResponse) { };
                posVelTicks = 0;

                //this begins the thread that looks for the canon camera response
                //when the image is returned, the image-to-trigger correlation must be done
                //we dont need to do anything else here
            }

            if (camera.ImageReady(out imageFilenameWithPath))
            {
                AppendRichText(richTextBox1, " camera Image:  " + imageFilenameWithPath + " dt=  " + elapsedTimeToTrigger.ToString() + "\r\n");
                camera.resetImageReady();
            }

            posVelTicks++;

 

 
        }



        private void triggerRequestThread_DoWork(object sender, DoWorkEventArgs e)
        {
            navIF_.SendCommandToMBed(NavInterfaceMBed.NAVMBED_CMDS.FIRE_TRIGGER);
            navIF_.WriteMessages(); //if we have messages to write (commands to the mbed) then write them  

            bool gettingTriggerResponse = true;

            while (gettingTriggerResponse)
            {
                //read the data received from the mbed to check for a PosVel message
                navIF_.ReadMessages();
                navIF_.ParseMessages();

                //if received, exit this loop and the receive message event will be fired
                if (navIF_.triggerTimeReceievdFromMbed)
                {
                    gettingTriggerResponse = false;
                    navIF_.triggerTimeReceievdFromMbed = false;
                }
            }
            waitingForTriggerResponse = false;

        }

        private void cameraImageReturned_DoWork(object sender, DoWorkEventArgs e)
        {
            //this thread is initiated at the instant that the trigger request is made
            timeFromTrigger.Start();
            while (!camera.ImageReady(out imageFilenameWithPath) ) {  }
            elapsedTimeToTrigger = timeFromTrigger.ElapsedMilliseconds;
            timeFromTrigger.Reset();
        }




     }  //end of Form1 Class

}  //end of the namespace
