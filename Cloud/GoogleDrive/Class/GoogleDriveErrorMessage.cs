﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cloud.GoogleDrive
{
  internal class GoogleDriveErrorMessage
  {
    public GDErrorMessage error;
  }

  internal class GDErrorMessage
  {
    public int code;
    public string message;
    public List<GDErrorreason> errors;

  }

  internal class GDErrorreason
  {
    public string domain;
    public string message;
    public string reason;
  }

}
