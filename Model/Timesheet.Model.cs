﻿//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;
//using System.IO;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Reflection;
//using System.Reflection.Emit;
//using System.Resources;
//using System.Runtime.Serialization;
//using System.Text;
//using System.Threading;
//using System.Xml.Linq;

//namespace Shared
//{
//    public partial class Timesheet
//    {
//    }
//}


////------------------------------------------------------------------------------
//// <auto-generated>
////     This code was generated from a template.
////
////     Changes to this file may cause incorrect behavior and will be lost if
////     the code is regenerated.
//// </auto-generated>
////------------------------------------------------------------------------------



//namespace SchoolSample.EntityModel
//{
//    public abstract class Person : IClientChangeTracking, IChangeTracking, IRevertibleChangeTracking,
//                                 INotifyDataErrorInfo, INodeLevel, IEditableObject
//    {
//        #region IClientChangeTracking Interface
    
//        /// <summary>
//        /// Accepts changes made to the entity object
//        /// </summary>
//        void IClientChangeTracking.AcceptChanges()
//        {
//            this.AcceptChanges();
//        }
    
//        /// <summary>
//        /// Rejects changes made to the entity object
//        /// </summary>
//        void IClientChangeTracking.RejectChanges()
//        {
//            this.RejectChanges();
//        }
    
//        /// <summary>
//        /// Returns whether the entity object has any changes
//        /// </summary>
//        public Boolean HasChanges
//        {
//            get { return _hasChanges; }
//            private set
//            {
//                if (_hasChanges != value)
//                {
//                    _hasChanges = value;
//                    if (_propertyChanged != null)
//                    {
//                        _propertyChanged(this, new PropertyChangedEventArgs("HasChanges"));
//                    }
//                }
//            }
//        }
//        private Boolean _hasChanges = true;
    
//        /// <summary>
//        /// Accepts changes made to the entity object and all objects of its object graph
//        /// </summary>
//        public virtual void AcceptObjectGraphChanges()
//        {
//            this.AcceptChanges();
//        }
    
//        /// <summary>
//        /// Rejects changes made to the entity object and all objects of its object graph
//        /// </summary>
//        public virtual void RejectObjectGraphChanges()
//        {
//            this.RejectChanges();
//        }
    
//        /// <summary>
//        /// Returns whether the entity object along with its object graph has any changes
//        /// </summary>
//        public bool ObjectGraphHasChanges()
//        {
//            var visitedGraph = new List<object>();
//            return ObjectGraphHasChanges(ref visitedGraph);
//        }
    
//        internal virtual bool ObjectGraphHasChanges(ref List<object> visitedGraph)
//        {
//            // if already visited this object, just return false
//            if (visitedGraph.Any(n => ReferenceEquals(n, this))) return false;
    
//            var hasChanges = HasChanges;
//            if (hasChanges) return true;
    
//            // if not, add itself to the visited graph
//            visitedGraph.Add(this);
//            return false;
//        }
    
//        internal virtual bool ObjectGraphHasChanges(Dictionary<INodeLevel, int> nodeLevelDictionary)
//        {
//            var hasChanges = HasChanges;
//            if (hasChanges) return true;
//            return false;
//        }
    
//        internal Dictionary<INodeLevel, int> GetNodeLevelDictionary()
//        {
//            int currentLevel = 1; // start with level one
//            var nodeLevelDictionary = new Dictionary<INodeLevel, int> { { this, currentLevel } };
//            bool foundNextLevelNodes;
        
//            do
//            {
//                var nextLevelNodes = new List<INodeLevel>();
//                var level = currentLevel;
//                // search for the next level nodes using what are currently in the nodeLevelDictionary
//                foreach (var n in nodeLevelDictionary.Where(i => i.Value == level))
//                {
//                    nextLevelNodes.AddRange(n.Key.GetNextLevelNodes(nodeLevelDictionary));
//                }
//                foundNextLevelNodes = nextLevelNodes.Count > 0;
//                if (foundNextLevelNodes)
//                {
//                    currentLevel++;
//                    foreach (var node in nextLevelNodes)
//                        nodeLevelDictionary.Add(node, currentLevel);
//                }
//            } while (foundNextLevelNodes);
        
//            return nodeLevelDictionary;
//        }
    
//        public virtual List<INodeLevel> GetNextLevelNodes(Dictionary<INodeLevel, int> nodeLevelDictionary)
//        {
//            var nextLevelNodes = new List<INodeLevel>();
    
//            return nextLevelNodes;
//        }
    
//        /// <summary>
//        /// Returns the estimate size of the entity object along with its object graph
//        /// </summary>
//        public long EstimateObjectGraphSize()
//        {
//            long size = 0;
//            var visitedGraph = new List<object>();
//            EstimateObjectGraphSize(ref size, ref visitedGraph);
//            return size;
//        }
    
//        internal virtual void EstimateObjectGraphSize(ref long size, ref List<object> visitedGraph)
//        {
//            // if already visited this object, just return
//            if (visitedGraph.Any(n => ReferenceEquals(n, this))) return;
    
//            size += EstimateSize;
//            // add itself to the visited graph
//            if (visitedGraph.All(i => !ReferenceEquals(i, this))) visitedGraph.Add(this);
//        }
    
//        /// <summary>
//        /// Returns the estimate size of the optimized entity object graph
//        /// with only objects that have changes
//        /// </summary>
//        public long EstimateObjectGraphChangeSize()
//        {
//            long size = 0;
//            if (!ObjectGraphHasChanges()) return size;
//            var nodeLevelDictionary = GetNodeLevelDictionary();
//            EstimateObjectGraphChangeSize(ref size, nodeLevelDictionary);
//            return size;
//        }
    
//        internal virtual void EstimateObjectGraphChangeSize(ref long size, Dictionary<INodeLevel, int> nodeLevelDictionary)
//        {
//            size += EstimateSize;
//        }
    
//        /// <summary>
//        /// Returns an optimized entity object graph with only objects that have changes
//        /// </summary>
//        public IObjectWithChangeTracker GetObjectGraphChanges()
//        {
//            if (!ObjectGraphHasChanges()) return null;
    
//            var item = this.Clone();
//            var nodeLevelDictionary = item.GetNodeLevelDictionary();
    
//            // loop through all navigation properties and trim any unchanged items
//            item.TrimUnchangedEntities(nodeLevelDictionary);
    
//            return item;
//        }
    
//        internal virtual void TrimUnchangedEntities(Dictionary<INodeLevel, int> nodeLevelDictionary)
//        {
//        }

//        #endregion

//        #region IClientChangeTracking Helper Property
    
//        internal virtual long EstimateSize
//        {
//            get
//            {
//                long _size = 0;
//                // estimate size of all Simple Properties
//                _size += sizeof(Int32);    // PersonId
//                if (Name != null)
//                    _size += Name.Length * sizeof(char);    // Name
//                _size += sizeof(Int32);    // Status
//                if (Version != null)
//                    _size += Version.Length * sizeof (Byte);    // Version
//                return _size;
//            }
//        }

//        #endregion

//        #region IChangeTracking and IRevertibleChangeTracking interfaces
    
//        void IChangeTracking.AcceptChanges()
//        {
//            this.AcceptChanges();
//        }
    
//        bool IChangeTracking.IsChanged
//        {
//            get { return HasChanges; }
//        }
    
//        void IRevertibleChangeTracking.RejectChanges()
//        {
//            this.RejectChanges();
//        }

//        #endregion

//        #region INotifyDataErrorInfo interface
    
//        private Dictionary<string, List<ValidationResult>> _validationErrors;
    
//        protected Dictionary<string, List<ValidationResult>> ValidationErrors
//        {
//            get { return _validationErrors ?? (_validationErrors = new Dictionary<string, List<ValidationResult>>()); }
//        }
    
//        protected event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        
//        /// <summary>
//        /// Raises the INotifyDataErrorInfo.ErrorsChanged event
//        /// </summary>
//        protected void RaiseErrorsChanged(string propertyName)
//        {
//            if (ErrorsChanged != null)
//            {
//                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
//            }
//        }
    
//        event EventHandler<DataErrorsChangedEventArgs> INotifyDataErrorInfo.ErrorsChanged
//        {
//            add
//            {
//                ErrorsChanged += value;
//            }
//            remove
//            {
//                ErrorsChanged -= value;
//            }
//        }
    
//        /// <summary>
//        /// Gets the currently known errors for the provided property name. Use String.Empty/null
//        /// to retrieve entity-level errors.
//        /// </summary>
//        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
//        {
//            if (propertyName == null)
//            {
//                propertyName = string.Empty;
//            }
    
//            if (ValidationErrors.ContainsKey(propertyName))
//            {
//                return ValidationErrors[propertyName];
//            }
//            return null;
//        }
    
//        /// <summary>
//        /// Gets a value indicating whether there are known errors or not.
//        /// </summary>
//        bool INotifyDataErrorInfo.HasErrors
//        {
//            get
//            {
//                return ValidationErrors.Keys.Count != 0;
//            }
//        }

//        #endregion

//        #region INotifyDataErrorInfo Protected & Private Helper Methods
    
//        /// <summary>
//        /// Declares a new error for the property name provided, or the entity if
//        /// propertyName is String.Empty/null.
//        /// </summary>
//        protected void AddError(string propertyName, ValidationResult validationResult)
//        {
//            if (validationResult == null)
//            {
//                throw new ArgumentNullException("validationResult");
//            }
    
//            if (propertyName == null)
//            {
//                propertyName = string.Empty;
//            }
    
//            List<ValidationResult> errors;
//            if (!ValidationErrors.ContainsKey(propertyName))
//            {
//                errors = new List<ValidationResult>();
//                ValidationErrors.Add(propertyName, errors);
//            }
//            else
//            {
//                errors = ValidationErrors[propertyName];
//            }
//            // search for duplicate error message with the same propertyName
//            var foundError = errors.FirstOrDefault(n => 
//                n.ErrorMessage == validationResult.ErrorMessage);
//            if (foundError == null)
//            {
//                errors.Insert(0, validationResult);
//                RaiseErrorsChanged(propertyName);
//            }
//        }
    
//        /// <summary>
//        /// Removes one specific error for the provided property name.
//        /// </summary>
//        /// <param name="propertyName"></param>
//        /// <param name="validationResult"></param>
//        protected void RemoveError(string propertyName, ValidationResult validationResult)
//        {
//            if (validationResult == null)
//            {
//                throw new ArgumentNullException("validationResult");
//            }
    
//            if (propertyName == null)
//            {
//                propertyName = string.Empty;
//            }
    
//            List<ValidationResult> errors;
//            if (ValidationErrors.ContainsKey(propertyName))
//            {
//                errors = ValidationErrors[propertyName];
//                // search for the error message that need to be removed
//                var foundError = errors.FirstOrDefault(n =>
//                    n.ErrorMessage == validationResult.ErrorMessage);
//                if (foundError != null)
//                {
//                    errors.Remove(foundError);
//                    if (errors.Count == 0)
//                    {
//                        // This entity no longer exposes errors for this property name.
//                        ValidationErrors.Remove(propertyName);
//                    }
//                    RaiseErrorsChanged(propertyName);
//                }
//            }
//        }
    
//        /// <summary>
//        /// Removes the known errors for the provided property name.
//        /// </summary>
//        /// <param name="propertyName">Property name or String.Empty/null for top-level errors</param>
//        protected void ClearErrors(string propertyName)
//        {
//            if (propertyName == null)
//            {
//                propertyName = string.Empty;
//            }
    
//            if (ValidationErrors.ContainsKey(propertyName))
//            {
//                // This entity no longer exposes errors for this property name.
//                ValidationErrors.Remove(propertyName);
//                RaiseErrorsChanged(propertyName);
//            }
//        }
    
//        /// <summary>
//        /// Removes the known errors for all property names.
//        /// </summary>
//        protected void ClearErrors()
//        {
//            foreach (string propertyName in ValidationErrors.Keys.ToList())
//            {
//                // This entity no longer exposes errors for this property name.
//                ValidationErrors.Remove(propertyName);
//                RaiseErrorsChanged(propertyName);
//            }
//        }
    
//        /// <summary>
//        /// Gets or sets a value indicating whether to suspend validation
//        /// whenever any entity property changes.
//        /// </summary>
//        public Boolean SuspendValidation;
    
//        /// <summary>
//        /// Gets a value indicating whether or not top-level validation rules
//        /// must be applied whenever any entity property changes.
//        /// </summary>
//        protected static Boolean ValidateEntityOnPropertyChanged;
    
//        /// <summary>
//        /// Removes any known errors for the provided property name
//        /// by calling ClearErrors()
//        /// </summary>
//        /// <param name="propertyName">Property name or String.Empty/null for top-level errors</param>
//        partial void PropertySetterEntry(string propertyName)
//        {
//            if (IsDeserializing || SuspendValidation)
//            {
//                return;
//            }
    
//            if (ValidateEntityOnPropertyChanged)
//            {
//                ClearErrors();
//            }
//            else
//            {
//                ClearErrors(propertyName);
//            }
//        }
    
//        /// <summary>
//        /// Validates for any known errors for the provided property name
//        /// </summary>
//        /// <param name="propertyName">Property name or String.Empty/null for top-level errors</param>
//        /// <param name="propertyValue">Property value</param>
//        partial void PropertySetterExit(string propertyName, object propertyValue)
//        {
//            if (IsDeserializing || SuspendValidation)
//            {
//                return;
//            }
    
//            if (ValidateEntityOnPropertyChanged)
//            {
//                Validate(string.Empty, this);
//            }
//            else
//            {
//                Validate(propertyName, propertyValue);
//            }
//        }

//        #endregion

//        #region IEditableObject interface
    
//        private Dictionary<string , object> _cache;
    
//        public virtual void BeginEdit()
//        {
//            if (_cache == null) _cache = new Dictionary<string, object>();
//            // copy all Simple Properties except the primary key fields
//            _cache["Name"] = Name;
//            _cache["Status"] = Status;
//            _cache["Version"] = Version;
//            // copy ChangeTracker
//            _cache["ChangeTracker"] = ChangeTracker.Clone();
//        }
    
//        public virtual void CancelEdit()
//        {
//            if (_cache == null) _cache = new Dictionary<string, object>();
//            if (_cache.Count == 0) return;
//            bool changeTrackingEnabled = ChangeTracker.ChangeTrackingEnabled;
//            this.StopTracking();
//            // copy all Simple Properties except the primary key fields
//            if (Name != (string)_cache["Name"])
//                Name = (string)_cache["Name"];
//            else
//                OnPropertyChanged("Name");
//            if (Status != (StatusEnum)_cache["Status"])
//                Status = (StatusEnum)_cache["Status"];
//            else
//                OnPropertyChanged("Status");
//            if (Version != (byte[])_cache["Version"])
//                Version = (byte[])_cache["Version"];
//            else
//                OnPropertyChanged("Version");
//            // copy ChangeTracker
//            ChangeTracker = (ObjectChangeTracker)_cache["ChangeTracker"];
//            ChangeTracker.ChangeTrackingEnabled = changeTrackingEnabled;
//            _cache.Clear();
//        }
    
//        public virtual void EndEdit()
//        {
//            if (_cache == null) _cache = new Dictionary<string, object>();
//            _cache.Clear();
//        }

//        #endregion

//    }
//}


