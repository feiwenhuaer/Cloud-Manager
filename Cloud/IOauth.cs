﻿using System;

namespace Cloud
{
    public delegate void UriResponse(Uri uri);
    public interface IOauth
    {
        string Url { set; }
        string CheckUrl { set; }
        void ShowUI(object owner);
        event UriResponse EventUriResponse;
        void CloseUI();
    }
}
