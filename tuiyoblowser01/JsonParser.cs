using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace tuiyoblowser01
{
    class JsonParser
    {
        public string getStingFromJSON(string source,string key) {
            JContainer json = JsonConvert.DeserializeObject<JContainer>(source);
            string s = json[key].ToString();
            return s;
        }

        public favorite getFavoriteFromJSON(string source)
        {
            favorite fv = new favorite();
            fv = JsonConvert.DeserializeObject<favorite>(source);
            return fv;            
        }

        /*
        public List<string[]> getStringListFromJSON(string source){
            JContainer json = JsonConvert.DeserializeObject<JContainer>(source);
            List<string[]> favlist = new List<string[]> { };
            

            return;
        }*/
        
    }
}
