using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace employeeForm
{
    public partial class empForm : Form
    {
        //employees file
        XmlDocument employeesFile = new XmlDocument();
        XmlNode employees;
        //file tags
        XmlNodeList names;
        XmlNodeList phones;
        XmlNodeList addresses;
        XmlNodeList emails;
        //file iterator to track the current employee
        int iterator;

        public empForm()
        {
            InitializeComponent();
        }

        private void empForm_Load(object sender, EventArgs e)
        {
            onload();
        }

        public void onload()
        {
            iterator = 0;
            //load the file
            employeesFile.Load(@"C:\Users\Blu-Ray\OneDrive\Desktop\iti\XML\lab-1\employee.xml");
            //get the root element
            employees = employeesFile.DocumentElement;
            //get the tag names in array lists
            names = employeesFile.GetElementsByTagName("name");
            phones = employeesFile.GetElementsByTagName("phones");
            addresses = employeesFile.GetElementsByTagName("addresses");
            emails = employeesFile.GetElementsByTagName("email");
            //fill the initial data
            fillData(iterator);
        }
        //filling the form data
        public void fillData(int i)
        {
            nameTextBox.Text = names[iterator].InnerText;
            phoneTextBox.Text = phones[iterator].FirstChild.InnerText;
            printAddress(i);
            emailTextBox.Text = emails[iterator].InnerText;
        }
        //adding the full address to the textbox
        private void printAddress(int i)
        {
            XmlNodeList address = addresses[i].FirstChild.ChildNodes;
            string fullAddress = "";
            foreach (XmlNode node in address)
            {
                fullAddress += node.InnerText + ", ";
            }
            fullAddress = fullAddress.Remove(fullAddress.Length - 2, 2);
            fullAddress = fullAddress.Insert(fullAddress.Length, ".");
            addressTextBox.Text = fullAddress;
        }

        //next, previous, update, insert, delete and search buttons
        private void nextBtn_Click(object sender, EventArgs e)
        {
            if (iterator < names.Count - 1)
            {
                iterator++;
            }
            fillData(iterator);
        }
        private void previousBtn_Click(object sender, EventArgs e)
        {
            if (iterator != 0)
            {
                iterator--;
            }
            fillData(iterator);
        }
        private void insertBtn_Click(object sender, EventArgs e)
        {
            //the last index of the added element
            int index = names.Count;
            //create an employee element cloned from current node
            //employee = employeesFile.GetElementsByTagName("employee")[0].CloneNode(true);

            XmlNode employee = createEmployeeElement();
            //append the new node to the file
            employees.AppendChild(employee);
            //assign the node values
            names[index].InnerText = nameTextBox.Text;
            phones[index].FirstChild.InnerText = phoneTextBox.Text;
            fillAddressElement(addressTextBox.Text, index);
            emails[index].InnerText = emailTextBox.Text;
            employeesFile.Save(@"C:\Users\Blu-Ray\OneDrive\Desktop\iti\XML\lab-1\employee.xml");
        }
        private void deleteBtn_Click(object sender, EventArgs e)
        {
            //get the current node
            XmlNode currentNode = employeesFile.GetElementsByTagName("employee")[iterator];
            employees.RemoveChild(currentNode);
            employeesFile.Save(@"C:\Users\Blu-Ray\OneDrive\Desktop\iti\XML\lab-1\employee.xml");
            onload();
        }
        private void updateBtn_Click(object sender, EventArgs e)
        {
            names[iterator].InnerText = nameTextBox.Text;
            phones[iterator].FirstChild.InnerText = phoneTextBox.Text;
            emails[iterator].InnerText = emailTextBox.Text;
            fillAddressElement(addressTextBox.Text, iterator);
            employeesFile.Save(@"C:\Users\Blu-Ray\OneDrive\Desktop\iti\XML\lab-1\employee.xml");
        }
        private void searchBtn_Click(object sender, EventArgs e)
        {
            bool Found = false;
            for (int i = 0; i < names.Count && !Found; i++)
            {
                if (string.Compare(names[i].InnerText, searchTextBox.Text, true) == 0)
                {
                    iterator = i;
                    fillData(i);
                    Found = true;
                }
            }
            if (!Found)
                MessageBox.Show("this name doesnot exist");
        }


        //creating an employee element with its sub nodes
        public XmlElement createEmployeeElement()
        {
            //employee element
            XmlNode employee = employeesFile.CreateElement("employee");
            //name element
            XmlNode name = employeesFile.CreateElement("name");
            //phones element
            XmlNode phones = employeesFile.CreateElement("phones");
            XmlNode phone = employeesFile.CreateElement("phone");
            XmlAttribute type = employeesFile.CreateAttribute("type");
            type.Value = "mobile";
            phone.Attributes.Append(type);
            //addresses element
            XmlNode addresses = employeesFile.CreateElement("addresses");
            XmlNode address = employeesFile.CreateElement("address");
            XmlNode street = employeesFile.CreateElement("street");
            XmlNode buildingNumber = employeesFile.CreateElement("buildingNumber");
            XmlNode region = employeesFile.CreateElement("region");
            XmlNode city = employeesFile.CreateElement("city");
            XmlNode country = employeesFile.CreateElement("country");
            //email element
            XmlNode email = employeesFile.CreateElement("email");

            //append these elements
            phones.AppendChild(phone);
            address.AppendChild(street);
            address.AppendChild(buildingNumber);
            address.AppendChild(region);
            address.AppendChild(city);
            address.AppendChild(country);
            addresses.AppendChild(address);
            employee.AppendChild(name);
            employee.AppendChild(phones);
            employee.AppendChild(addresses);
            employee.AppendChild(email);

            return (XmlElement)employee;
        }
        //parsing the address
        public void fillAddressElement(string addressText, int i)
        {
            string[] add = addressText.Split(',');
            XmlNode addressElement = addresses[i].FirstChild;
            XmlNodeList addressElements = addressElement.ChildNodes;
            //assigning the address elements from the parsed string of form address
            addressElements[0].InnerText = add[0];
            for (int j = 1; j < 4; j++)
            {
                addressElements[j].InnerText = add[j].Remove(0, 1);
            }
            addressElements[4].InnerText = add[4].Remove(add[4].Length - 1, 1).Remove(0, 1);
        }


        //adding placeholder to the search box
        private void removePlaceHolder(object sender, EventArgs e)
        {
            if (searchTextBox.Text == "Search by name")
            {
                searchTextBox.Text = "";

            }
        }
        private void addPlaceHolder(object sender, EventArgs e)
        {
            if (searchTextBox.Text == "")
                searchTextBox.Text = "Search by name";
        }

        private void showBrowserBtn_Click(object sender, EventArgs e)
        {
            ////Create a new XslTransform object.
            //XslCompiledTransform xslt = new XslCompiledTransform();

            ////Load the stylesheet.
            //xslt.Load(@"C:\Users\Blu-Ray\OneDrive\Desktop\iti\XML\lab-3\empStyling.xsl");

            ////Transform the data and send the output to the console.
            //xslt.Transform(@"C:\Users\Blu-Ray\OneDrive\Desktop\iti\XML\lab-1\employee.xml",
            //    @"C:\Users\Blu-Ray\OneDrive\Desktop\iti\XML\lab-3\employee.xml");

            //System.Diagnostics.Process.Start(@"C:\Users\Blu-Ray\OneDrive\Desktop\iti\XML\lab 3\employee.xml");
            System.Diagnostics.Process.Start("IExplore.exe", @"C:\Users\Blu-Ray\OneDrive\Desktop\iti\XML\lab-1\employee.xml");
        }
    }
}
