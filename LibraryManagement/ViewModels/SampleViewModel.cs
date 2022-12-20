using LibraryManagement.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LibraryManagement.ViewModels
{
    public class SampleViewModel : BaseViewModel
    {
        // List of Paramater loaded from database
        private ObservableCollection<Paramater> paramatersList;
        // Property for encapsulation
        public ObservableCollection<Paramater> ParamatersList { 
            get => paramatersList; 
            set { 
                paramatersList = value; 
                OnPropertyChanged();
            }
        }

        // Add command to add a new paramater to DB
        public ICommand AddCommand;

        /// <summary>
        /// Constructor of class
        /// </summary>
        public SampleViewModel()
        {
            // Retrieve data from DB
            RetrieveData();

            // Define AddCommand
            AddCommand = new AppCommand<object>(
                p => true,
                p =>
                {
                    // Create new Paramater object
                    var paramater = new Paramater { nameParameter = "NoExpireDay", valueParameter = 10 };
                    // Add it to DB context
                    DataAdapter.Instance.DB.Paramaters.Add(paramater);
                    // Save changes to real DB
                    DataAdapter.Instance.DB.SaveChanges();
                });
        }

        /// <summary>
        /// Retrive Paramaters collection from DB
        /// </summary>
        private void RetrieveData()
        {
            // Load paramaters collections from DB
            var List = DataAdapter.Instance.DB.Paramaters;
            // Create a new instance of ObservableColleion with List as constructor's paramater.
            paramatersList = new ObservableCollection<Paramater>(List);
        }
  
    }
}
