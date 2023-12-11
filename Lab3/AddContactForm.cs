using System;
using System.Drawing;
using System.Windows.Forms;

public class AddContactForm : Form
{
    private TextBox _firstNameTextBox;
    private TextBox _lastNameTextBox;
    private TextBox _phoneTextBox;
    private Button _uploadPhotoButton;
    private PictureBox _photoPictureBox;
    private Button _addButton;

    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Phone { get; private set; }
    public string PhotoFilePath { get; private set; }

    public AddContactForm()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        _firstNameTextBox = new TextBox { PlaceholderText = "First Name", Dock = DockStyle.Top };
        _lastNameTextBox = new TextBox { PlaceholderText = "Last Name", Dock = DockStyle.Top };
        _phoneTextBox = new TextBox { PlaceholderText = "Phone", Dock = DockStyle.Top };
        _uploadPhotoButton = new Button { Text = "Upload Photo", Dock = DockStyle.Top };
        _uploadPhotoButton.Click += UploadPhotoButton_Click;

        _photoPictureBox = new PictureBox { Dock = DockStyle.Top, Height = 100 };
        _addButton = new Button { Text = "Add Contact", Dock = DockStyle.Top };
        _addButton.Click += AddButton_Click;

        // Set up layout
        Controls.Add(_phoneTextBox);
        Controls.Add(_lastNameTextBox);
        Controls.Add(_firstNameTextBox);
        Controls.Add(_uploadPhotoButton);
        Controls.Add(_photoPictureBox);
        Controls.Add(_addButton);

        // Set up form
        Text = "Add Contact";
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

    private void AddButton_Click(object sender, EventArgs e)
    {
        FirstName = _firstNameTextBox.Text;
        LastName = _lastNameTextBox.Text;
        Phone = _phoneTextBox.Text;
        PhotoFilePath = _photoPictureBox.ImageLocation;

        DialogResult = DialogResult.OK;
        Close();
    }
}

