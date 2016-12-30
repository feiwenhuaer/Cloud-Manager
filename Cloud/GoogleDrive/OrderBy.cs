namespace Cloud.GoogleDrive
{
    public static class OrderBy
    {
        public static string Get(OrderByEnum[] orderby, int desc = -1)
        {
            string s = "";
            int sub_desc = 0;
            foreach (OrderByEnum e in orderby)
            {
                if (desc == -1) s += e.ToString() + ",";
                else if (desc == sub_desc) s += e.ToString() + " desc,";
                else s += e.ToString() + ",";
                sub_desc++;
            }
            return s.TrimEnd(',');
        }
    }
}
