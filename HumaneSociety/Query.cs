using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HumaneSociety
{
    public static class Query
    {        
        static HumaneSocietyDataContext db;

        static Query()
        {
            db = new HumaneSocietyDataContext();
        }

        internal static List<USState> GetStates()
        {
            List<USState> allStates = db.USStates.ToList();       

            return allStates;
        }
            
        internal static Client GetClient(string userName, string password)
        {
            Client client = db.Clients.Where(c => c.UserName == userName && c.Password == password).Single();

            return client;
        }

        internal static List<Client> GetClients()
        {
            List<Client> allClients = db.Clients.ToList();

            return allClients;
        }

        internal static void AddNewClient(string firstName, string lastName, string username, string password, string email, string streetAddress, int zipCode, int stateId)
        {
            Client newClient = new Client();

            newClient.FirstName = firstName;
            newClient.LastName = lastName;
            newClient.UserName = username;
            newClient.Password = password;
            newClient.Email = email;

            Address addressFromDb = db.Addresses.Where(a => a.AddressLine1 == streetAddress && a.Zipcode == zipCode && a.USStateId == stateId).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if (addressFromDb == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = streetAddress;
                newAddress.City = null;
                newAddress.USStateId = stateId;
                newAddress.Zipcode = zipCode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                addressFromDb = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            newClient.AddressId = addressFromDb.AddressId;

            db.Clients.InsertOnSubmit(newClient);

            db.SubmitChanges();
        }

        internal static void UpdateClient(Client clientWithUpdates)
        {
            // find corresponding Client from Db
            Client clientFromDb = null;

            try
            {
                clientFromDb = db.Clients.Where(c => c.ClientId == clientWithUpdates.ClientId).Single();
            }
            catch(InvalidOperationException e)
            {
                Console.WriteLine("No clients have a ClientId that matches the Client passed in.");
                Console.WriteLine("No update have been made.");
                return;
            }
            
            // update clientFromDb information with the values on clientWithUpdates (aside from address)
            clientFromDb.FirstName = clientWithUpdates.FirstName;
            clientFromDb.LastName = clientWithUpdates.LastName;
            clientFromDb.UserName = clientWithUpdates.UserName;
            clientFromDb.Password = clientWithUpdates.Password;
            clientFromDb.Email = clientWithUpdates.Email;

            // get address object from clientWithUpdates
            Address clientAddress = clientWithUpdates.Address;

            // look for existing Address in Db (null will be returned if the address isn't already in the Db
            Address updatedAddress = db.Addresses.Where(a => a.AddressLine1 == clientAddress.AddressLine1 && a.USStateId == clientAddress.USStateId && a.Zipcode == clientAddress.Zipcode).FirstOrDefault();

            // if the address isn't found in the Db, create and insert it
            if(updatedAddress == null)
            {
                Address newAddress = new Address();
                newAddress.AddressLine1 = clientAddress.AddressLine1;
                newAddress.City = null;
                newAddress.USStateId = clientAddress.USStateId;
                newAddress.Zipcode = clientAddress.Zipcode;                

                db.Addresses.InsertOnSubmit(newAddress);
                db.SubmitChanges();

                updatedAddress = newAddress;
            }

            // attach AddressId to clientFromDb.AddressId
            clientFromDb.AddressId = updatedAddress.AddressId;
            
            // submit changes
            db.SubmitChanges();
        }
        
        internal static void AddUsernameAndPassword(Employee employee)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.EmployeeId == employee.EmployeeId).FirstOrDefault();

            employeeFromDb.UserName = employee.UserName;
            employeeFromDb.Password = employee.Password;

            db.SubmitChanges();
        }

        internal static Employee RetrieveEmployeeUser(string email, int employeeNumber)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.Email == email && e.EmployeeNumber == employeeNumber).FirstOrDefault();

            if (employeeFromDb == null)
            {
                throw new NullReferenceException();
            }
            else
            {
                return employeeFromDb;
            }
        }

        internal static Employee EmployeeLogin(string userName, string password)
        {
            Employee employeeFromDb = db.Employees.Where(e => e.UserName == userName && e.Password == password).FirstOrDefault();

            return employeeFromDb;
        }

        internal static bool CheckEmployeeUserNameExist(string userName)
        {
            Employee employeeWithUserName = db.Employees.Where(e => e.UserName == userName).FirstOrDefault();

            return employeeWithUserName == null;
        }


        //// TODO Items: ////
        
        // TODO: Allow any of the CRUD operations to occur here
        internal static void RunEmployeeQueries(Employee employee, string crudOperation)
        {
            crudOperation = crudOperation.ToLower();
            //Need switch statement for all four CRUD operations
            switch(crudOperation)
            {
                case "create":
                    db.Employees.InsertOnSubmit(employee);
                    break;
                case "read":
                    db.Employees.Select(x => x.EmployeeNumber == employee.EmployeeNumber);
                    break;
                case "update":
                    List<Employee> empUpdate = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).ToList();
                    db.Employees.InsertOnSubmit(empUpdate[0]);
                    //Without question, not the right way to do this. But it seems to work.
                    break;
                case "delete":
                    List<Employee> empDelete = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).ToList();
                    db.Employees.DeleteOnSubmit(empDelete[0]);
                    //Again, probably wrong, but works
                    break;
            }
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            throw new NotImplementedException();
        }

        internal static Animal GetAnimalByID(int id)
        {
            throw new NotImplementedException();
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {            
            throw new NotImplementedException();
        }

        internal static void RemoveAnimal(Animal animal)
        {
            throw new NotImplementedException();
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IQueryable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            //Dictionary values are each of the numbers they've chosen and the values they entered. Need to choose by each of the 8 options, check for the value they put in,
            //and see if it matches any or multiple animals, then return any and all animals who match the search criteria
            //How to do it without adding until all search criteria are checked? I could do it, but it would add any qualifiers, then add more qualifiers, not check all at once
            //A bunch of && statements? 8 of them?

            //Might be good to use switch statement to take whatever their choices are in the dictionary
            List<int> keyList = new List<int>(updates.Keys);
            //Seems like there's a better way to do this. Check if you have time
            var matchingAnimals = db.Animals.ToList();

            foreach (KeyValuePair<int, string> pair in updates)
            {
                switch (pair.Key)
                {
                    case 1:
                        matchingAnimals.RemoveAll(a => a.Category.ToString() != pair.Value);
                        //Want it to be not equal. Probably need to do some kind of conversion or something.
                        //Doesn't quite work because it's testing each one against all of the values. I only want the values at 1, which is what they chose in this case
                        break;
                    case 2:
                        matchingAnimals.RemoveAll(a => a.Name.ToString() != pair.Value);
                        //Don't want where. I want a function to remove the items I don't want, that way I can use the same variable each time, and it will narrow as it moves down the list
                        break;
                    case 3:
                        matchingAnimals.RemoveAll(a => a.Age.ToString() != pair.Value);
                        break;
                    case 4:
                        matchingAnimals.RemoveAll(a => a.Demeanor.ToString() != pair.Value);
                        break;
                    case 5:
                        matchingAnimals.RemoveAll(a => a.KidFriendly.ToString() != pair.Value);
                        break;
                    case 6:
                        matchingAnimals.RemoveAll(a => a.PetFriendly.ToString() != pair.Value);
                        break;
                    case 7:
                        matchingAnimals.RemoveAll(a => a.Weight.ToString() != pair.Value);
                        break;
                    case 8:
                        matchingAnimals.RemoveAll(a => a.AnimalId.ToString() != pair.Value);
                        break;
                }
            }
            return matchingAnimals;

            //Right now it returns all animals in db. Need to use this function to narrow down by only the ones they've selected
            //Need here to be able to search by name and type, or name and id, etc. Want a list or something? How to choose from all the possibilities? 72 of them
            //This is the data I need I think. Each possibility has a number assigned

            //"1. Category", "2. Name", "3. Age", "4. Demeanor", "5. Kid friendly", "6. Pet friendly", "7. Weight", "8. ID", "9. Finished" 

            //case "1":
            //        searchParameters.Add(1, GetStringData("category", "the animal's"));
            //break;
            //    case "2":
            //        searchParameters.Add(2, GetStringData("name", "the animal's"));
            //break;
            //    case "3":
            //        searchParameters.Add(3, GetIntegerData("age", "the animal's").ToString());
            //break;
            //    case "4":
            //        searchParameters.Add(4, GetStringData("demeanor", "the animal's"));
            //break;
            //    case "5":
            //        searchParameters.Add(5, GetBitData("the animal", "kid friendly").ToString());
            //break;
            //    case "6":
            //        searchParameters.Add(6, GetBitData("the animal", "pet friendly").ToString());
            //break;
            //    case "7":
            //        searchParameters.Add(7, GetIntegerData("weight", "the animal's").ToString());
            //break;
            //    case "8":
            //        searchParameters.Add(8, GetIntegerData("ID", "the animal's").ToString());
            //break;
            //default:
            //        DisplayUserOptions("Input not recognized please try agian");
            //break;
        }

        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            throw new NotImplementedException();
        }
        
        internal static Room GetRoom(int animalId)
        {
            throw new NotImplementedException();
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            throw new NotImplementedException();
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            throw new NotImplementedException();
        }

        internal static IQueryable<Adoption> GetPendingAdoptions()
        {
            throw new NotImplementedException();
        }

        internal static void UpdateAdoption(bool isAdopted, Adoption adoption)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveAdoption(int animalId, int clientId)
        {
            throw new NotImplementedException();
        }

        // TODO: Shots Stuff
        internal static IQueryable<AnimalShot> GetShots(Animal animal)
        {
            throw new NotImplementedException();
        }

        internal static void UpdateShot(string shotName, Animal animal)
        {
            throw new NotImplementedException();
        }
    }
}