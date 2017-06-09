using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudManagerGeneralLib.Class
{
    public static class CalTimeLeft
    {
        public static int TimeRefresh = 500;//refresh in 500 ms
        public static void CalSpeedAndTimeLeft(this Transfer transfer,long Group_TotalTransfer = -1)
        {
            long time_milisec_group = CurrentMillis.Millis - transfer.TimeStamp;
            if (time_milisec_group != 0 & time_milisec_group >= TimeRefresh)
            {
                TransferItem item = transfer as TransferItem;
                TransferGroup group = transfer as TransferGroup;
                if (group != null && Group_TotalTransfer < 0) throw new Exception("Group_TotalTransfer < 0");
                
                long SizeWasTransfer = (item != null ? item.SizeWasTransfer : Group_TotalTransfer);

                transfer.TimeStamp = CurrentMillis.Millis;
                decimal speed = ((decimal)(SizeWasTransfer - transfer.OldTransfer)) * 1000 / time_milisec_group;
                transfer.OldTransfer = SizeWasTransfer;
                transfer.DataSource.Speed = UnitConventer.ConvertSize(speed, 2, UnitConventer.unit_speed);
                if (speed != 0)
                {
                    long TotalSize = (item != null ? item.From.node.Info.Size : group.TotalFileLength);
                    long length_left = TotalSize - SizeWasTransfer;
                    long TimeLeft_sec = length_left / decimal.ToInt64(speed);
                    transfer.DataSource.Estimated = CurrentMillis.GetTimeBySecond(TimeLeft_sec);
                }
            }
        }
    }
}
