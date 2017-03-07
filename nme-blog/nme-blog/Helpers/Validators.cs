using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace nme_blog.Helpers
{
    public class Validators
    {
        public static string GetPathToImage(HttpPostedFileBase image)
        {
            string fileName = "";
            if (ImageUploadValidator.IsWebFriendlyImage(image))
            {
                fileName = Path.GetFileName(image.FileName);
                image.SaveAs(Path.Combine(HttpContext.Current.Server.MapPath("~/Uploads/"), fileName));
            }
            return fileName;
        }

    }
}