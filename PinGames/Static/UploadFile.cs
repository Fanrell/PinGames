using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using PinGames.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PinGames.Static
{
    public static class UploadFile
    {
        

        internal static async Task<string> UploadGameCover(IWebHostEnvironment webHost, GameToUpload model)
        {
            string imgName = null;
            if(model.GameImg != null)
            {
                imgName = Guid.NewGuid().ToString() + "_" + model.Name.Replace(" ","_") + "." + model.GameImg.FileName.Split(".")[1];
                var filePath = Path.Combine(webHost.WebRootPath, "img", "Game", imgName);
                using(var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.GameImg.CopyToAsync(fileStream);
                }
            }
            return imgName;
        }

        internal static async Task<string> UploadProfileImg(IWebHostEnvironment webHost, ProfileInfoModel profileData)
        {
            string imgName = null;
            if(profileData.profilePicture != null)
            {
                imgName = Guid.NewGuid().ToString() + "_" + profileData.profilePicture.FileName;
                var filePath = Path.Combine(webHost.WebRootPath, "img", "Profile", imgName);
                using(var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await profileData.profilePicture.CopyToAsync(fileStream);
                }
            }
            return imgName;
        }
    }
}
