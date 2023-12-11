using System;
using System.Drawing;
using System.Windows.Forms;

public class UpdateContactForm : Form
{
    private TextBox _firstNameTextBox;
    private TextBox _lastNameTextBox;
    private TextBox _phoneTextBox;
    private Button _uploadPhotoButton;
    private PictureBox _photoPictureBox;
    private Button _updateButton;

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Phone { get; private set; }
    public string PhotoFilePath { get; private set; }

    public UpdateContactForm(PersonEntity contact)
    {
        InitializeComponents();

        // Populate form with existing contact information
        _firstNameTextBox.Text = contact.FirstName;
        _lastNameTextBox.Text = contact.LastName;
        _phoneTextBox.Text = contact.Phone;
        _photoPictureBox.ImageLocation = contact.PhotoUrl;
    }

    private void InitializeComponents()
    {
        _firstNameTextBox = new TextBox { PlaceholderText = "First Name", Dock = DockStyle.Top };
        _lastNameTextBox = new TextBox { PlaceholderText = "Last Name", Dock = DockStyle.Top };
        _phoneTextBox = new TextBox { PlaceholderText = "Phone", Dock = DockStyle.Top };

        _uploadPhotoButton = new Button { Text = "Upload New Photo", Dock = DockStyle.Top };
        _uploadPhotoButton.Click += UploadPhotoButton_Click;

        _photoPictureBox = new PictureBox { Dock = DockStyle.Top, Height = 100 };
        _updateButton = new Button { Text = "Update Contact", Dock = DockStyle.Top };
        _updateButton.Click += UpdateButton_Click;

        // Set up layout
        Controls.Add(_phoneTextBox);
        Controls.Add(_lastNameTextBox);
        Controls.Add(_firstNameTextBox);
        Controls.Add(_uploadPhotoButton);
        Controls.Add(_photoPictureBox);
        Controls.Add(_updateButton);

        // Set up form
        Text = "Update Contact";
        Width = 300;
        Height = 300;  // Increased height to accommodate the photo
        FormBorderStyle = FormBorderStyle.FixedDialog;
        StartPosition = FormStartPosition.CenterScreen;
    }

    private void UploadPhotoButton_Click(object sender, EventArgs e)
    {
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.Filter = "Image Files (*.bmp;*.jpg;*.jpeg;*.gif;*.png)|*.bmp;*.jpg;*.jpeg;*.gif;*.png";
            openFileDialog.Title = "Select an Image";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                _photoPictureBox.ImageLocation = openFileDialog.FileName;
            }
        }
    }

    private void UpdateButton_Click(object sender, EventArgs e)
    {
        FirstName = _firstNameTextBox.Text;
        LastName = _lastNameTextBox.Text;
        Phone = _phoneTextBox.Text;
        PhotoFilePath = _photoPictureBox.ImageLocation;

        DialogResult = DialogResult.OK;
        Close();
    }
}
