using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloud.GoogleDrive
{
  public class DriveAPIHttprequestv3: DriveApiHttprequest
  {
    public DriveAPIHttprequestv3(TokenGoogleDrive token, GD_LimitExceededDelegate LimitExceeded = null):base(token,LimitExceeded)
    {
      version = "v3/";
    }


    public string About_Get()
    {
      return Request<string>("about", TypeRequest.GET).DataTextResponse;
    }

    public class About
    {
      DriveApiHttprequest api;
      public About(DriveApiHttprequest api)
      {
        this.api = api;
      }


    }
  }
}
