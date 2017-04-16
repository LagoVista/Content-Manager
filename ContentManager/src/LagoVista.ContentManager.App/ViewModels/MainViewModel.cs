using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LagoVista.Core;
using Microsoft.WindowsAzure.Storage; // Namespace for CloudStorageAccount
using Microsoft.WindowsAzure.Storage.Table; // Namespace for Table storage types
using System.Collections.ObjectModel;
using System.ComponentModel;
using LagoVista.Core.Commanding;
using LagoVista.Core.ViewModels;

namespace LagoVista.ContentManager.App.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        CloudTable _table;
        CloudTable _edits;
        CloudTableClient _client;
        public MainViewModel()
        {
            TextResources = new ObservableCollection<Models.TextResource>();

            ApproveCommand = new RelayCommand(Approve, CanUpdate);
            SaveCommand = new RelayCommand(Save, CanUpdate);
        }

        ObservableCollection<Models.TextResource> _textResources;
        public ObservableCollection<Models.TextResource> TextResources
        {
            get { return _textResources; }
            set { Set(ref _textResources, value); }
        }

        public string StorageKey { get; set; }
        public string StorageName { get; set; }

        Models.TextResource _textResource;
        public Models.TextResource TextResource
        {
            get { return _textResource; }
            set
            {
                Set(ref _textResource, value);
                if (value != null)
                {
                    TextResourceCopy = new Models.TextResourceCopy()
                    {
                        CreationDate = value.CreationDate,
                        ETag = value.ETag,
                        LastUpdatedDate = value.LastUpdatedDate,
                        Notes = value.Notes,
                        PartitionKey = value.PartitionKey,
                        RowKey = value.RowKey,
                        Text = value.Text,
                        OriginalText = value.Text,
                        Merged = false,
                        Timestamp = value.Timestamp
                    };

                    CanEdit = true;
                }
                else
                {
                    TextResourceCopy = null;
                    CanEdit = false;
                }

                SaveCommand.RaiseCanExecuteChanged();
                ApproveCommand.RaiseCanExecuteChanged();
            }
        }

        Models.TextResourceCopy _textResourceCopy;
        public Models.TextResourceCopy TextResourceCopy
        {
            get { return _textResourceCopy; }
            set
            {
                Set(ref _textResourceCopy, value);
            }
        }

        private bool _savingRecord;
        public bool NetworkConnectionActive
        {
            get { return _savingRecord; }
            set { Set(ref _savingRecord, value); }
        }

        public async Task PopulateResourcesAsync()
        {
            _client = new CloudTableClient(new Uri($"https://{StorageName}.table.core.windows.net"), new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(StorageName,StorageKey));
            _table = _client.GetTableReference("allresources");
            _edits = _client.GetTableReference("editedresources");

            NetworkConnectionActive = true;

            var operation = new TableQuery<Models.TextResource>().Where(TableQuery.CombineFilters(
                    TableQuery.GenerateFilterCondition("Edits", QueryComparisons.Equal, "false"),
                    TableOperators.And,
                    TableQuery.GenerateFilterCondition("Approved", QueryComparisons.Equal, "false")));

            TableContinuationToken tableContinuationToken = null;

            var response = await _table.ExecuteQuerySegmentedAsync(operation, tableContinuationToken);

            TextResources = response.Results.ToObservableCollection();
            NetworkConnectionActive = false;
        }

        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand ApproveCommand { get; private set; }

        public bool CanUpdate()
        {
            return TextResourceCopy != null;
        }

        private bool _canEdit = false;
        public bool CanEdit
        {
            get { return _canEdit; }
            set { Set(ref _canEdit, value); }
        }

        public async void Save()
        {
            if (TextResource != null)
            {
                NetworkConnectionActive = true;
                var addChangedOperation = TableOperation.InsertOrReplace(TextResourceCopy);
                TextResourceCopy.CreationDate = DateTime.Now;
                TextResourceCopy.LastUpdatedDate = DateTime.Now;
                await _edits.ExecuteAsync(addChangedOperation);

                TextResource.Edits = true;
                TextResource.LastUpdatedDate = DateTime.Now;

                var replaceOperation = TableOperation.Replace(TextResource);
                await _table.ExecuteAsync(replaceOperation);

                TextResources.Remove(TextResource);

                TextResource = null;
                TextResourceCopy = null;
                NetworkConnectionActive = false;
            }
        }

        public async void Approve()
        {
            if (TextResource != null)
            {
                NetworkConnectionActive = true;
                var operation = TableOperation.InsertOrReplace(TextResource);
                TextResource.LastUpdatedDate = DateTime.Now;
                TextResource.Approved = true;
                TextResources.Remove(TextResource);
                await _table.ExecuteAsync(operation);
                TextResource = null;
                TextResourceCopy = null;
                NetworkConnectionActive = false;
            }
        }
    }
}
