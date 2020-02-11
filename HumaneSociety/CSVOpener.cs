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
            List<string> tempPlaceholder = new List<string>() {System.IO.File.ReadAllText(@"C:\Users\Public\TestFolder\WriteText.txt")};
            List<Animal> animalList = new List<Animal>();

            var listOfListsFromCSV = tempPlaceholder.Select(s => s.Split(',').Select(Convert.ToString).ToList().ToList()).ToList();

            foreach (List<string> list in listOfListsFromCSV)
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
                            animal.Age = Convert.ToInt32(list[i]);
                            break;
                        case 3:
                            animal.Demeanor = list[i].ToString();
                            break;
                        case 4:
                            animal.KidFriendly = Convert.ToBoolean(list[i]);
                            break;
                        case 5:
                            animal.PetFriendly = Convert.ToBoolean(list[i]);
                            break;
                        case 6:
                            animal.Gender = list[i].ToString();
                            break;
                        case 7:
                            animal.AdoptionStatus = list[i].ToString();
                            break;
                        case 8:
                            animal.CategoryId = Convert.ToInt32(list[i]);
                            break;
                        case 9:
                            animal.DietPlanId = Convert.ToInt32(list[i]);
                            break;
                        case 10:
                            animal.EmployeeId = Convert.ToInt32(list[i]);
                            break;
                    }
                }
                animalList.Add(animal);       
            }
            return animalList;
        }
    }
}
