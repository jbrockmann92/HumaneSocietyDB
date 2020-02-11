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
                case "remove":
                    List<Employee> empRemove = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).ToList();
                    db.Employees.DeleteOnSubmit(empRemove[0]);
                    //Again, probably wrong, but works
                    break;
                case "update":
                    List<Employee> empUpdate = db.Employees.Where(e => e.EmployeeNumber == employee.EmployeeNumber).ToList();
                    db.Employees.InsertOnSubmit(empUpdate[0]);
                    //Without question, not the right way to do this. But it seems to work.
                    //FirstOrDefault() is what you want here
                    break;
                case "display":
                    db.Employees.Select(x => x.EmployeeNumber == employee.EmployeeNumber);
                    break;
            }
        }

        // TODO: Animal CRUD Operations
        internal static void AddAnimal(Animal animal)
        {
            db.Animals.InsertOnSubmit(animal);
        }

        internal static Animal GetAnimalByID(int id)
        {
            db.Animals.Select(a => a.AnimalId == animal.AnimalId);
            return Animal;
        }

        internal static void UpdateAnimal(int animalId, Dictionary<int, string> updates)
        {
            db.Animals.InsertOnSubmit(db.Animals.Where(a => a.AnimalId == animal.AnimalId).ToList());
        }

        internal static void RemoveAnimal(Animal animal)
        {
            db.Animals.DeleteOnSubmit(animal);
        }
        
        // TODO: Animal Multi-Trait Search
        internal static IEnumerable<Animal> SearchForAnimalsByMultipleTraits(Dictionary<int, string> updates) // parameter(s)?
        {
            var matchingAnimals = db.Animals.ToList();

            foreach (KeyValuePair<int, string> pair in updates)
            {
                switch (pair.Key)
                {
                    case 1:
                        matchingAnimals.RemoveAll(a => a.Category.Name != pair.Value);
                        break;
                    case 2:
                        matchingAnimals.RemoveAll(a => a.Name.ToString() != pair.Value);
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
        }

        // TODO: Misc Animal Things
        internal static int GetCategoryId(string categoryName)
        {
            //Need to take in the categoryName and return the categoryId. Test the name against the category, and return the categoryId
            string stringId = db.Animals.Where(a => a.Category.Name.Equals(categoryName)).ToString();
            int categoryId = int.Parse(stringId);
            return categoryId;
            //Rewrite if time
        }
        
        internal static Room GetRoom(int animalId)
        {
            //Returns Room object when given an animal's id number
            var animalRoom = db.Rooms.Where(r => r.AnimalId.Equals(animalId)).FirstOrDefault();
            return animalRoom;
        }
        
        internal static int GetDietPlanId(string dietPlanName)
        {
            var planID = db.DietPlans.Where(d => d.Name.Equals(dietPlanName)).FirstOrDefault();
            return planID.DietPlanId;
        }

        // TODO: Adoption CRUD Operations
        internal static void Adopt(Animal animal, Client client)
        {
            //Want to change animal's status to adopted, but what about client? change their animals +1?
            //JOIN Animal and Client on Adoption table, check to make sure variables match, then approve?
            //Remove animal from table

            Adoption adoption = new Adoption();
            adoption.ClientId = client.ClientId;
            adoption.AnimalId = animal.AnimalId;
            adoption.ApprovalStatus = "Not Adopted";
            adoption.AdoptionFee = 100;
            adoption.PaymentCollected = false;

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