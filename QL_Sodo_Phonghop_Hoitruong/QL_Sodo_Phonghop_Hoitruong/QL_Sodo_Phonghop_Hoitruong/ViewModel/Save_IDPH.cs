using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QL_Sodo_Phonghop_Hoitruong.ViewModel
{
    class Save_Data
    {
        private static Save_Data _instance;

        public int idphienhop { get; set; }
        public int length { get; set; }
        public int width { get; set; }
        public int soday { get; set; }
        public int sohangmoiday { get; set; }
        public string soghemoihang { get; set; }
        public string hinhthuc { get; set; }

        private Save_Data()
        {
        }
        

        public static Save_Data Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Save_Data();
                }
                return _instance;
            }
        }
        
    }
   
}
