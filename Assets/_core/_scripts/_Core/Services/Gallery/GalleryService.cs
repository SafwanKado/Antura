using System;
using UnityEngine;

namespace Antura.Core.Services.Gallery
{
    public class GalleryService
    {
        public GalleryService()
        {
            DetectWriteAccess();
        }

        public bool HasWriteAccess;

        public void DetectWriteAccess()
        {
            var permission = NativeGallery.CheckPermission(NativeGallery.PermissionType.Write);
            HasWriteAccess = (permission == NativeGallery.Permission.ShouldAsk || permission == NativeGallery.Permission.Granted);
            if (!HasWriteAccess) Debug.LogWarning("Has no write access to the gallery!");
        }

        public bool SaveScreenshot(Texture2D texture)
        {
            if (!HasWriteAccess) return true;
            try
            {
                NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(texture, "Antura", "AnturaSpace" + (Time.time / 1000) + ".png", (success, path) => Debug.Log("Media save result: " + success + " " + path));
                Debug.Log("SaveScreenshot Permission result: " + permission);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception during gallery saving: " + ex.Message);
                return false;
            }

            //if (HasWriteAccess) {
            //    // TODO need better way to generate incremental / unique images filenames

            //    NativeGallery.SaveImageToGallery(texture, "Antura", "AnturaSpace" + (Time.time / 1000) + ".png");
            //}
        }
    }
}