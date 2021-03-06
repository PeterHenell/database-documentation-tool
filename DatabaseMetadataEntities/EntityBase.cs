﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DatabaseMetadata.Entities
{
    public partial class EntityBase : System.ComponentModel.INotifyPropertyChanged
    {
        public string Level1Name { get; set; }
        public string Level2Name { get; set; }
        public string Schema { get; set; }
        public Level1Types Level1Type { get; set; }
        public Level2Types Level2Type { get; set; }


        string _description = string.Empty;
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {

                if (value != _description)
                {
                    _description = value;
                    HasChanges = true;
                }
            }
        }

        public bool IsNewDescription { get; set; }

        bool _hasChanges = false;
        public bool HasChanges { get { return _hasChanges; } set { _hasChanges = value; } }



       

        public SaveModes GetSaveMode()
        {
            SaveModes sm = SaveModes.NoAction;

            if (!HasChanges)
            {
                return sm;
            }

            // Figure out if the description is new or if it exists and should be updated.
            if (IsNewDescription)
                sm = SaveModes.New;
            else
                sm = SaveModes.Update;
            // If the Description is empty then we should drop the extended property
            if (string.IsNullOrEmpty(Description))
            {
                sm = SaveModes.Delete;
            }

            return sm;
        }

        public EntityBase(string level1Name, string level2Name, string schema, string description, bool isNewDescription)
        {
            Level1Name = level1Name;
            Level2Name = level2Name;
            Schema = schema;
            _description = description; // use direct access so that it does not set the HasChanges flag
            IsNewDescription = isNewDescription;
        }
        //private EntityBase()
        //{

        //}

        //public EntityBase Create(string name, string schema, string description, bool isNewDescription) 
        //{
        //    EntityBase entity = new EntityBase(name, schema, description, isNewDescription);
        //    return entity;

        //}


        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

    }
}
