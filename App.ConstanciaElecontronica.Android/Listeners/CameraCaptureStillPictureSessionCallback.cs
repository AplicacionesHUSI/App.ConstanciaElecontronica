﻿using Android.Hardware.Camera2;
using Android.Util;
using App.ConstanciaElecontronica.Camera;

namespace App.ConstanciaElecontronica.Droid.Listeners
{
    public class CameraCaptureStillPictureSessionCallback : CameraCaptureSession.CaptureCallback
    {
        private static readonly string TAG = "CameraCaptureStillPictureSessionCallback";

        private readonly Camera2BasicFragment owner;

        public CameraCaptureStillPictureSessionCallback(Camera2BasicFragment owner)
        {
            if (owner == null)
                throw new System.ArgumentNullException("owner");
            this.owner = owner;
        }

        public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result)
        {
            // If something goes wrong with the save (or the handler isn't even 
            // registered, this code will toast a success message regardless...)
            owner.ShowToast("Saved: " + owner.mFile);
            Log.Debug(TAG, owner.mFile.ToString());
            //owner.UnlockFocus();
            CameraPage.CAMPage.completePick();


        }
    }
}