using System.Collections.Generic;
using System.Threading;

namespace SupDataDll
{

    public class ManagerThread
    {
        public List<Thread> delete = new List<Thread>();
        public List<Thread> rename = new List<Thread>();
        public List<Thread> createfolder = new List<Thread>();

        public void CleanThr()
        {
            Clean(delete);
            Clean(rename);
            Clean(createfolder);
        }

        private void Clean(List<Thread> threads)
        {
            for (int i = 0; i < threads.Count; i++)
            {
                if (!threads[i].IsAlive)
                {
                    threads.RemoveAt(i);
                    i--;
                }
            }
        }

        public void CloseAll()
        {
            Close(delete);
            Close(rename);
            Close(createfolder);
        }

        void Close(List<Thread> threads)
        {
            foreach (Thread thr in threads)
            {
                if (thr != null && thr.IsAlive) try { thr.Abort(); } catch { }
            }
        }
    }
}
