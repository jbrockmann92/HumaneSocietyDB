using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    class CSVOpener
    { 

        //Should create a list of animals by the end of this class

        public List<List<string>> LoadCSV()
        {
            List<string> tempPlaceholder = new List<string>() { "Hank,50,5,Playful,1,1,Male,Not Adopted,1,4,2", "Lady,14,3,Funny,1,0,Female,Not Adopted,2,1,4" };
            List<List<string>> listOfLists = new List<List<string>>();

            var listOfListsFromCSV = tempPlaceholder.Select(s => s.Split(',').Select(Convert.ToString).ToList().ToList()).ToList();
            //Three ToLists in a row. Not bad
            //Maybe want something in this class that will convert each item based on what it contains??? Not ideal, but not sure besides that

            foreach (List<string> list in listOfListsFromCSV)
            {
                foreach (string item in list)
                {

                }
            }
            //Probably should be able to do with LINQ. Fix later if time


            //intList = classGrades.Select(s => s.Split(',').Select(int.Parse).ToList()).ToList()

            return listOfLists;
        }
    }
}
