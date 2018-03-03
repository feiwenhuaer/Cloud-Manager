using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudManagerGeneralLib.Class
{
  public static class CalTimeLeft
  {
    public static int TimeRefresh = 500;//refresh in 500 ms
    public static void CalSpeedAndTimeLeft(this TransferItem transfer)
    {
      long mili = CurrentMillis.Millis;
      if (mili - transfer.TimeStamp < 500) return;
      decimal speed = ((decimal)(transfer.SizeWasTransfer - transfer.OldTransfer)) * 1000 / (mili - transfer.TimeStamp);
      transfer.OldTransfer = transfer.SizeWasTransfer;
      transfer.DataSource.Speed = UnitConventer.ConvertSize(speed, 2, UnitConventer.unit_speed);
      if (speed != 0)
      {
        long length_left = transfer.From.node.Info.Size - transfer.SizeWasTransfer;
        long TimeLeft_sec = length_left / decimal.ToInt64(speed);
        transfer.DataSource.Estimated = CurrentMillis.GetTimeBySecond(TimeLeft_sec);
      }
      else transfer.DataSource.Estimated = "";
    }
  }
}

