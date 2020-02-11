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

        public List<Animal> LoadCSV()
        {
            List<string> tempPlaceholder = new List<string>() { "Hank,50,5,Playful,1,1,Male,Not Adopted,1,4,2", "Lady,14,3,Funny,1,0,Female,Not Adopted,2,1,4" };
            List<List<string>> listOfLists = new List<List<string>>();
            List<Animal> animalList;

            var listOfListsFromCSV = tempPlaceholder.Select(s => s.Split(',').Select(Convert.ToString).ToList().ToList()).ToList();
            //Three ToLists in a row. Not bad
            //Maybe want something in this class that will convert each item based on what it contains??? Not ideal, but not sure besides that
            //Make a list of some class that 3 classes inherit from? bool, int, string all custom? Not ideal

            foreach (List<string> list in listOfLists)
            {
                Animal animal = new Animal();
                for (int i = 0; i < list.Count; i++)
                //Switch statement that takes each of the 11 items and converts it to the proper datatype, then assigns it to an animal object. Then add that animal to a list of animals and return it
                {
                    switch (i)
                    {
                        case 0:
                            animal.Name = list[i].ToString();
                            break;
                        case 1:
                            animal.Weight = Convert.ToInt32(list[i]);
                            break;
                        case 2:
                            animal.Name = list[i].ToString();
                            break;
                        case 3:
                            animal.Name = list[i].ToString();
                            break;
                        case 4:
                            animal.Name = list[i].ToString();
                            break;
                        case 5:
                            animal.Name = list[i].ToString();
                            break;
                        case 6:
                            animal.Name = list[i].ToString();
                            break;
                        case 7:
                            animal.Name = list[i].ToString();
                            break;
                        case 8:
                            animal.Name = list[i].ToString();
                            break;
                        case 9:
                            animal.Name = list[i].ToString();
                            break;
                        case 10:
                            animal.Name = list[i].ToString();
                            break;
                        case 11:
                            animal.Name = list[i].ToString();
                            break;
                    }
                }
                        
                
            }

            return animalList;
        }
    }
}
