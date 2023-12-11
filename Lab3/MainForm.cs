using Azure.Data.Tables;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public class MainForm : Form
{
    private AzureBlobStorageService _azureBlobStorageService;
    private AzureTableStorageService _azureTableStorageService;

    private ListView _contactsListView;
    private Button _addButton;
    private Button _updateButton;
    private Button _deleteButton;
    private ImageList _contactImageList = new ImageList();

    public MainForm(AzureBlobStorageService blobStorageService, AzureTableStorageService tableStorageService)
    {
        _azureBlobStorageService = blobStorageService ?? throw new ArgumentNullException(nameof(blobStorageService));
        _azureTableStorageService = tableStorageService ?? throw new ArgumentNullException(nameof(tableStorageService));

        // Initialize UI components
        _contactsListView = new ListView
        {
            View = View.Details,
            Columns = { "First Name", "Last Name", "Phone" },
            Dock = DockStyle.Fill,
            SmallImageList = _contactImageList
        };

        _addButton = new Button
        {
            Text = "Add Contact",
            Dock = DockStyle.Bottom
        };
        _addButton.Click += AddButton_Click;

        _updateButton = new Button
        {
            Text = "Update Contact",
            Dock = DockStyle.Bottom
        };
        _updateButton.Click += UpdateButton_Click;

        _deleteButton = new Button
        {
            Text = "Delete Contact",
            Dock = DockStyle.Bottom
        };
        _deleteButton.Click += DeleteButton_Click;

        var refreshButton = new Button
        {
            Text = "Refresh",
            Dock = DockStyle.Bottom
        };
        refreshButton.Click += RefreshButton_Click;

        // Set up layout
        Controls.Add(_contactsListView);
        Controls.Add(_addButton);
        Controls.Add(_updateButton);
        Controls.Add(_deleteButton);
        Controls.Add(refreshButton);

        LoadContacts();
    }

    private void RefreshButton_Click(object sender, EventArgs e)
    {
        LoadContacts();
    }

    private async void AddButton_Click(object sender, EventArgs e)
    {
        using (var addContactForm = new AddContactForm())
        {
            if (addContactForm.ShowDialog() == DialogResult.OK)
            {
                // Get user input from the AddContactForm
                string firstName = addContactForm.FirstName;
                string lastName = addContactForm.LastName;
                string phone = addContactForm.Phone;
                string photoFilePath = addContactForm.PhotoFilePath;
                string guid = Guid.NewGuid().ToString();

                // Upload photo to Azure Blob Storage
                string photoUrl = _azureBlobStorageService.UploadPhoto(photoFilePath, guid);

                // Create a new contact entity
                PersonEntity newContact = new PersonEntity
                {
                    PartitionKey = "Contact",
                    RowKey = guid,
                    FirstName = firstName,
                    LastName = lastName,
                    Phone = phone,
                    PhotoUrl = photoUrl
                };

                // Add the new contact to Azure Table Storage
                _azureTableStorageService.AddEntity(newContact);

                // Load the updated contact list
                LoadContacts();
            }
        }
    }
    private void UpdateButton_Click(object sender, EventArgs e)
    {
        if (_contactsListView.SelectedItems.Count > 0)
        {
            var selectedContact = (PersonEntity)_contactsListView.SelectedItems[0].Tag;

            using (var updateContactForm = new UpdateContactForm(selectedContact))
            {
                if (updateContactForm.ShowDialog() == DialogResult.OK)
                {
                    string updatedFirstName = updateContactForm.FirstName;
                    string updatedLastName = updateContactForm.LastName;
                    string updatedPhone = updateContactForm.Phone;
                    string updatedPhotoFilePath = updateContactForm.PhotoFilePath;

                    selectedContact.FirstName = updatedFirstName;
                    selectedContact.LastName = updatedLastName;
                    selectedContact.Phone = updatedPhone;

                    _azureTableStorageService.UpdateEntity(selectedContact);
                    if (!string.IsNullOrEmpty(updatedPhotoFilePath))
                    {
                        _azureBlobStorageService.DeletePhoto(selectedContact.RowKey);
                        string updatedPhotoUrl = _azureBlobStorageService.UploadPhoto(updatedPhotoFilePath, selectedContact.RowKey);
                        selectedContact.PhotoUrl = updatedPhotoUrl;
                        _azureTableStorageService.UpdateEntity(selectedContact);
                    }
                    LoadContacts();
                }
            }
        }
        else
        {
            MessageBox.Show("Please select a contact to update.", "Update Contact", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
    private void DeleteButton_Click(object sender, EventArgs e)
    {
        if (_contactsListView.SelectedItems.Count > 0)
        {
            var selectedContact = (PersonEntity)_contactsListView.SelectedItems[0].Tag;

            // Confirm deletion with the user
            var result = MessageBox.Show($"Are you sure you want to delete {selectedContact.FirstName} {selectedContact.LastName}?", "Delete Contact", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Delete the contact entity from Azure Table Storage
                _azureTableStorageService.DeleteEntity(selectedContact.PartitionKey, selectedContact.RowKey);

                // Delete the contact's photo from Azure Blob Storage
                _azureBlobStorageService.DeletePhoto(selectedContact.RowKey);

                // Load the updated contact list
                LoadContacts();
            }
        }
        else
        {
            MessageBox.Show("Please select a contact to delete.", "Delete Contact", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void LoadContacts()
    {
        _contactsListView.Items.Clear();
        _contactImageList.Images.Clear();

        List<PersonEntity> contacts = _azureTableStorageService.GetEntities("Contact");

        foreach (var contact in contacts)
        {
            byte[] photoData = _azureBlobStorageService.DownloadPhoto(contact.RowKey);
            Image photo = ByteArrayToImage(photoData);
            var item = new ListViewItem(new[] { contact.FirstName, contact.LastName, contact.Phone });
            item.Tag = contact;
            if (photo != null)
            {
                if (!_contactImageList.Images.ContainsKey(contact.RowKey))
                {
                    _contactImageList.Images.Add(contact.RowKey, photo);
                }
                item.ImageKey = contact.RowKey;
            }
            _contactsListView.Items.Add(item);
        }
    }

    private Image ByteArrayToImage(byte[] byteArray)
    {
        if (byteArray == null || byteArray.Length == 0)
        {
            // Handle the case where byteArray is null or empty
            // For example, return a default image or null, depending on your requirements
            return null;
        }

        using (MemoryStream ms = new MemoryStream(byteArray))
        {
            return Image.FromStream(ms);
        }
    }
}
