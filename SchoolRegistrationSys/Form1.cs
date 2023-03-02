using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Json.Nodes;
using RestSharp;

namespace SchoolRegistrationSys
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //Trigger to display search from data table. This will disable view button and hide controls for adding new student/teacher
        private void view_clicked(object sender, EventArgs e)
        {
            viewTable.Enabled = false;
            addTable.Enabled = true;

            label1.Visible = true;
            label2.Visible = true;
            label3.Visible = true;
            designationMenu.Visible = true;
            idInput.Visible = true;
            firstNameInput.Visible = true;
            searchBttn.Visible = true;

            label4.Visible = false;
            label5.Visible = false;
            label6.Visible = false;
            label7.Visible = false; //Birthday
            label8.Visible = false; //Adviser
            label9.Visible = false; //Old GPA
            designationAdd.Visible = false;
            textBox1.Visible = false;
            textBox2.Visible = false;
            dateTimePicker1.Visible = false;
            textBox4.Visible = false; //Adviser
            textBox5.Visible = false; //Old GPA

            button1.Visible = false;
            button2.Visible = false;

            label10.Visible = false;
            textBox6.Visible = false;
            checkBox1.Visible = false;
        }

        private void designation_updated(object sender, EventArgs e)
        {
            if (designationAdd.Text == "Student")
            {
                addTable.Enabled = false;
                viewTable.Enabled = true;

                label1.Visible = false;
                label2.Visible = false;
                label3.Visible = false;
                designationMenu.Visible = false;
                idInput.Visible = false;
                firstNameInput.Visible = false;
                searchBttn.Visible = false;

                label4.Visible = true;
                label5.Visible = true;
                label6.Visible = true;
                label7.Visible = true; //Birthday
                label8.Visible = true; //Adviser
                label9.Visible = true; //Old GPA
                designationAdd.Visible = true;
                textBox1.Visible = true;
                textBox2.Visible = true;
                dateTimePicker1.Visible = true;
                textBox4.Visible = true; //Adviser
                textBox5.Visible = true; //Old GPA

                button1.Visible = true;
                button2.Visible = true;

                label10.Visible = false;
                textBox6.Visible = false;
                checkBox1.Visible = false;
            } else
            {
                addTable.Enabled = false;
                viewTable.Enabled = true;

                label1.Visible = false;
                label2.Visible = false;
                label3.Visible = false;
                designationMenu.Visible = false;
                idInput.Visible = false;
                firstNameInput.Visible = false;
                searchBttn.Visible = false;

                label4.Visible = true;
                label5.Visible = true;
                label6.Visible = true;
                label7.Visible = true; //Birthday
                label8.Visible = false; //Adviser
                label9.Visible = false; //Old GPA
                designationAdd.Visible = true;
                textBox1.Visible = true;
                textBox2.Visible = true;
                dateTimePicker1.Visible = true;
                textBox4.Visible = false; //Adviser
                textBox5.Visible = false; //Old GPA

                button1.Visible = true;
                button2.Visible = true;

                label10.Visible = true;
                textBox6.Visible = true;
                checkBox1.Visible = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Initialization
            BindingList<Person> listPerson = new BindingList<Person>();
            dateTimePicker1.Value = DateTime.Today;

            string jsonValue = jsonVal();

            //Parse all of the Json
            try
            {
                JsonNode doc = JsonNode.Parse(jsonValue);

                // Get a subsection and deserialize it into an array.
                JsonArray basketsAvail = doc!["baskets"]!.AsArray()!;
                int jsonSize = basketsAvail.Count;
                string dataBasket = "";

                var options = new RestClientOptions("https://getpantry.cloud")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                string nameStr = "";

                foreach (JsonNode idName in basketsAvail)
                {
                    nameStr = idName["name"].ToString();

                    string newEndpnt = "/apiv1/pantry/b8035d04-9c01-45af-b810-0a1826ac54b9/basket/" + nameStr;

                    var request = new RestRequest(newEndpnt, Method.Get);
                    RestResponse responseBasket = client.Execute(request);

                    dataBasket = responseBasket.Content;
                }

                string boolHolder;
                int i;

                using JsonDocument docBasket = JsonDocument.Parse(dataBasket);
                JsonElement root = docBasket.RootElement;

                for (i = 0; i < jsonSize; i++)
                {
                    boolHolder = root.GetProperty("isStarSection").ToString();

                    try
                    {
                        //Checks value of type to see if student because default value is student
                        string dsgntn = root.GetProperty("type").ToString();
                        if (dsgntn == "Student")
                        {
                            //Fills class PersonDetail through list add
                            listPerson.Add(
                                new Person
                                {
                                    id = nameStr,
                                    type = dsgntn,
                                    firstName = root.GetProperty("firstName").ToString(),
                                    LastName = root.GetProperty("lastName").ToString(),
                                    Birthday = root.GetProperty("birthday").ToString(),
                                    Age = root.GetProperty("age").ToString(),
                                    adviser = root.GetProperty("adviser").ToString(),
                                    oldGpa = root.GetProperty("oldGpa").ToString(),
                                    isStarSection = System.Convert.ToBoolean(boolHolder),
                                    handledStudents = root.GetProperty("handledStudents").ToString()
                                });
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                form1_PersonDataGridView.DataSource = listPerson;
            } catch
            {
                Console.WriteLine("No record exists.");
            }
        }

        //Function to get string data from json call
        static string jsonVal()
        {
            var options = new RestClientOptions("https://getpantry.cloud")
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            var request = new RestRequest("/apiv1/pantry/b8035d04-9c01-45af-b810-0a1826ac54b9", Method.Get);
            RestResponse response = client.Execute(request);

            string data = response.Content;

            return data; //returns initial rest call (getting list of id's)
        }

        public class Person
        {
            public string id { get; set; }
            public string type { get; set; }
            public string firstName { get; set; }
            public string LastName { get; set; }
            public string Birthday { get; set; }
            public string Age { get; set; }
            public string adviser { get; set; }
            public string oldGpa { get; set; }
            public bool isStarSection { get; set; }
            public string handledStudents { get; set; }
        }

        private void search_clicked(object sender, EventArgs e)
        {
            string dsgntnMenu = designationMenu.Text;
            string idInpt = idInput.Text;
            string fName = firstNameInput.Text;

            BindingList<Person> listPerson = new BindingList<Person>(); //initialization

            string jsonValue = jsonVal();

            //Parse all of the Json
            JsonNode doc = JsonNode.Parse(jsonValue);

            // Get a subsection and deserialize it into an array.
            JsonArray basketsAvail = doc!["baskets"]!.AsArray()!;
            int jsonSize = basketsAvail.Count;
            string dataBasket = "";

            var options = new RestClientOptions("https://getpantry.cloud")
            {
                MaxTimeout = -1,
            };
            var client = new RestClient(options);
            string nameStr = "";

            foreach (JsonNode idName in basketsAvail)
            {
                nameStr = idName["name"].ToString();

                string newEndpnt = "/apiv1/pantry/b8035d04-9c01-45af-b810-0a1826ac54b9/basket/" + nameStr;

                var request = new RestRequest(newEndpnt, Method.Get);
                RestResponse responseBasket = client.Execute(request);

                dataBasket = responseBasket.Content;
            }

            string boolHolder;
            int i;
            try
            {
                using JsonDocument docBasket = JsonDocument.Parse(dataBasket);
                JsonElement root = docBasket.RootElement;

                for (i = 0; i < jsonSize; i++)
                {
                    var u1 = root;
                    boolHolder = u1.GetProperty("isStarSection").ToString();

                    try
                    {
                        //Checks value of designation to filter table
                        string dsgntn = u1.GetProperty("type").ToString();
                        string idNum = nameStr;
                        string fNameVal = u1.GetProperty("firstName").ToString();

                        //Check if ID and name has a value. If not, do not consider it
                        if (dsgntn == dsgntnMenu && idInpt == "" && fName == "")
                        {
                            //Fills class PersonDetail through list add
                            listPerson.Add(
                                new Person
                                {
                                    id = idNum,
                                    type = dsgntn,
                                    firstName = fNameVal,
                                    LastName = u1.GetProperty("lastName").ToString(),
                                    Birthday = u1.GetProperty("birthday").ToString(),
                                    Age = u1.GetProperty("age").ToString(),
                                    adviser = u1.GetProperty("adviser").ToString(),
                                    oldGpa = u1.GetProperty("oldGpa").ToString(),
                                    isStarSection = System.Convert.ToBoolean(boolHolder),
                                    handledStudents = u1.GetProperty("handledStudents").ToString()
                                });
                        }
                        else if (dsgntn == dsgntnMenu && idNum == idInpt && fName == "")
                        {
                            listPerson.Add(
                                new Person
                                {
                                    id = idNum,
                                    type = dsgntn,
                                    firstName = fNameVal,
                                    LastName = u1.GetProperty("lastName").ToString(),
                                    Birthday = u1.GetProperty("birthday").ToString(),
                                    Age = u1.GetProperty("age").ToString(),
                                    adviser = u1.GetProperty("adviser").ToString(),
                                    oldGpa = u1.GetProperty("oldGpa").ToString(),
                                    isStarSection = System.Convert.ToBoolean(boolHolder),
                                    handledStudents = u1.GetProperty("handledStudents").ToString()
                                });
                        }
                        else if (dsgntn == dsgntnMenu && idInpt == "" && fNameVal.Contains(fName))
                        {
                            listPerson.Add(
                                new Person
                                {
                                    id = idNum,
                                    type = dsgntn,
                                    firstName = fNameVal,
                                    LastName = u1.GetProperty("lastName").ToString(),
                                    Birthday = u1.GetProperty("birthday").ToString(),
                                    Age = u1.GetProperty("age").ToString(),
                                    adviser = u1.GetProperty("adviser").ToString(),
                                    oldGpa = u1.GetProperty("oldGpa").ToString(),
                                    isStarSection = System.Convert.ToBoolean(boolHolder),
                                    handledStudents = u1.GetProperty("handledStudents").ToString()
                                });
                        }
                        else if (dsgntn == dsgntnMenu && idNum == idInpt && fNameVal.Contains(fName))
                        {
                            listPerson.Add(
                                new Person
                                {
                                    id = idNum,
                                    type = dsgntn,
                                    firstName = fNameVal,
                                    LastName = u1.GetProperty("lastName").ToString(),
                                    Birthday = u1.GetProperty("birthday").ToString(),
                                    Age = u1.GetProperty("age").ToString(),
                                    adviser = u1.GetProperty("adviser").ToString(),
                                    oldGpa = u1.GetProperty("oldGpa").ToString(),
                                    isStarSection = System.Convert.ToBoolean(boolHolder),
                                    handledStudents = u1.GetProperty("handledStudents").ToString()
                                });
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                form1_PersonDataGridView.DataSource = listPerson;
            } catch
            {
                MessageBox.Show("No record exists.");
            }

            if (dsgntnMenu == "Student")
            {
                form1_PersonDataGridView.Columns[9].Visible = false; //handledStudents
                form1_PersonDataGridView.Columns[6].Visible = true; //adviser
                form1_PersonDataGridView.Columns[7].Visible = true; //oldGpa
            } else
            {
                form1_PersonDataGridView.Columns[9].Visible = true; //handledStudents
                form1_PersonDataGridView.Columns[6].Visible = false; //adviser
                form1_PersonDataGridView.Columns[7].Visible = false; //oldGpa
            }
        }

        private void create_record(object sender, EventArgs e)
        {
            if (personId.Text == "")
            {
                //Get current date
                DateTime today = DateTime.Today;
                string currDate = today.ToString("MM/dd/yyyy");

                //Compare with inputted date to get age
                TimeSpan diff1 = today.Subtract(dateTimePicker1.Value);
                int years = (int)diff1.TotalDays;
                years = years / 365;

                //Check if star section
                bool starSecVal = false;
                if (designationAdd.Text == "Student")
                {
                    int grade = 95;
                    try
                    {
                        grade = Int32.Parse(textBox5.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Please populate all fields.");
                    }
                    if (grade > 95)
                    {
                        starSecVal = true;
                    }
                    else
                    {
                        starSecVal = checkBox1.Checked;
                    }
                }

                //Call API to check id's on record
                string nameStr = "";
                int validId = 1;

                var options = new RestClientOptions("https://getpantry.cloud")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/apiv1/pantry/b8035d04-9c01-45af-b810-0a1826ac54b9", Method.Get);
                try
                {
                    RestResponse response = client.Execute(request);

                    string data = response.Content;

                    //Parse all of the Json
                    JsonNode doc = JsonNode.Parse(data);

                    // Get a subsection and deserialize it into an array.
                    JsonArray basketsAvail = doc!["baskets"]!.AsArray()!;
                    int jsonSize = basketsAvail.Count;

                    foreach (JsonNode idName in basketsAvail)
                    {
                        nameStr = idName["name"].ToString();

                        if (nameStr == validId.ToString())
                        {
                            validId++;
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("No record exists yet.");
                }

                string newEndpnt = "/apiv1/pantry/b8035d04-9c01-45af-b810-0a1826ac54b9/basket/" + validId.ToString();

                var requestAdd = new RestRequest(newEndpnt, Method.Post);
                requestAdd.RequestFormat = DataFormat.Json;
                requestAdd.AddJsonBody(new
                {
                    type = designationAdd.Text,
                    firstName = textBox1.Text,
                    lastName = textBox2.Text,
                    birthday = dateTimePicker1.Value.ToString(),
                    age = years.ToString(),
                    adviser = textBox4.Text,
                    oldGpa = textBox5.Text,
                    isStarSection = starSecVal,
                    handledStudents = textBox6.Text
                });
                RestResponse responseBasket = client.Execute(requestAdd);

                if (responseBasket.StatusCode.ToString() == "OK")
                {
                    BindingList<Person> listPerson = new BindingList<Person>();

                    listPerson.Add(
                                    new Person
                                    {
                                        id = validId.ToString(),
                                        type = designationAdd.Text,
                                        firstName = textBox1.Text,
                                        LastName = textBox2.Text,
                                        Birthday = dateTimePicker1.Value.ToString(),
                                        Age = years.ToString(),
                                        adviser = textBox4.Text,
                                        oldGpa = textBox5.Text,
                                        isStarSection = starSecVal,
                                        handledStudents = textBox6.Text
                                    });

                    form1_PersonDataGridView.DataSource = listPerson;
                }
            } else
            {
                //Get current date
                DateTime today = DateTime.Today;
                string currDate = today.ToString("MM/dd/yyyy");

                //Compare with inputted date to get age
                TimeSpan diff1 = today.Subtract(dateTimePicker1.Value);
                int years = (int)diff1.TotalDays;
                years = years / 365;

                //Check if star section
                bool starSecVal = false;
                if (designationAdd.Text == "Student")
                {
                    int grade = 95;
                    try
                    {
                        grade = Int32.Parse(textBox5.Text);
                    }
                    catch
                    {
                        MessageBox.Show("Please populate all fields.");
                    }
                    if (grade > 95)
                    {
                        starSecVal = true;
                    }
                    else
                    {
                        starSecVal = checkBox1.Checked;
                    }
                }

                //Trigger patch on json records
                var options = new RestClientOptions("https://getpantry.cloud")
                {
                    MaxTimeout = -1,
                };
                var client = new RestClient(options);
                var request = new RestRequest("/apiv1/pantry/b8035d04-9c01-45af-b810-0a1826ac54b9/basket/" + personId.Text, Method.Put);
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new
                {
                    type = designationAdd.Text,
                    firstName = textBox1.Text,
                    lastName = textBox2.Text,
                    birthday = dateTimePicker1.Value.ToString(),
                    age = years.ToString(),
                    adviser = textBox4.Text,
                    oldGpa = textBox5.Text,
                    isStarSection = starSecVal,
                    handledStudents = textBox6.Text
                });
                RestResponse response = client.Execute(request);

                //Update data grid view
                BindingList<Person> listPerson = new BindingList<Person>();

                string jsonValue = jsonVal();

                //Parse all of the Json
                try
                {
                    JsonNode doc = JsonNode.Parse(jsonValue);

                    // Get a subsection and deserialize it into an array.
                    JsonArray basketsAvail = doc!["baskets"]!.AsArray()!;
                    int jsonSize = basketsAvail.Count;
                    string dataBasket = "";

                    var optionsPatch = new RestClientOptions("https://getpantry.cloud")
                    {
                        MaxTimeout = -1,
                    };
                    var clientPatch = new RestClient(optionsPatch);
                    string nameStr = "";

                    foreach (JsonNode idName in basketsAvail)
                    {
                        nameStr = idName["name"].ToString();

                        string newEndpnt = "/apiv1/pantry/b8035d04-9c01-45af-b810-0a1826ac54b9/basket/" + nameStr;

                        var requestPatch = new RestRequest(newEndpnt, Method.Get);
                        RestResponse responseBasket = clientPatch.Execute(requestPatch);

                        dataBasket = responseBasket.Content;
                    }

                    string boolHolder;
                    int i;

                    using JsonDocument docBasket = JsonDocument.Parse(dataBasket);
                    JsonElement root = docBasket.RootElement;

                    for (i = 0; i < jsonSize; i++)
                    {
                        boolHolder = root.GetProperty("isStarSection").ToString();

                        try
                        {
                            //Checks value of type to see if student because default value is student
                            string dsgntn = root.GetProperty("type").ToString();
                            if (dsgntn == "Student")
                            {
                                //Fills class PersonDetail through list add
                                listPerson.Add(
                                    new Person
                                    {
                                        id = nameStr,
                                        type = dsgntn,
                                        firstName = root.GetProperty("firstName").ToString(),
                                        LastName = root.GetProperty("lastName").ToString(),
                                        Birthday = root.GetProperty("birthday").ToString(),
                                        Age = root.GetProperty("age").ToString(),
                                        adviser = root.GetProperty("adviser").ToString(),
                                        oldGpa = root.GetProperty("oldGpa").ToString(),
                                        isStarSection = System.Convert.ToBoolean(boolHolder),
                                        handledStudents = root.GetProperty("handledStudents").ToString()
                                    });
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                        }
                    }
                    form1_PersonDataGridView.DataSource = listPerson;
                }
                catch
                {
                    Console.WriteLine("No record exists.");
                }

                //Refresh
                personId.Text = "";
                button1.Text = "Add";
                button2.Text = "Delete";

                designationAdd.Text = "Student";
                textBox1.Text = "";
                textBox2.Text = "";
                dateTimePicker1.Value = DateTime.Today;
                textBox4.Text = "";
                textBox5.Text = "";
                checkBox1.Checked = false;
                textBox6.Text = "";
            }
        }

        //Event when delete button is clicked
        private void button2_Click(object sender, EventArgs e)
        {
            if (personId.Text == "")
            {
                foreach (DataGridViewRow r in form1_PersonDataGridView.SelectedRows)
                {
                    string id = r.Cells[0].Value.ToString(); //get id

                    //delete from json
                    var options = new RestClientOptions("https://getpantry.cloud")
                    {
                        MaxTimeout = -1,
                    };
                    var client = new RestClient(options);
                    var request = new RestRequest("/apiv1/pantry/b8035d04-9c01-45af-b810-0a1826ac54b9/basket/" + id, Method.Delete);
                    RestResponse response = client.Execute(request);
                    if (response.StatusCode.ToString() == "OK") //delete success
                    {
                        //delete from table
                        form1_PersonDataGridView.Rows.RemoveAt(r.Index);
                    }
                }
            } else
            {
                personId.Text = "";
                button1.Text = "Add";
                button2.Text = "Delete";

                designationAdd.Text = "Student";
                textBox1.Text = "";
                textBox2.Text = "";
                dateTimePicker1.Value = DateTime.Today;
                textBox4.Text = "";
                textBox5.Text = "";
                checkBox1.Checked = false;
                textBox6.Text = "";
            }
        }

        private void row_update(object sender, DataGridViewCellEventArgs e)
        {
            foreach (DataGridViewRow r in form1_PersonDataGridView.SelectedRows)
            {
                personId.Text = r.Cells[0].Value.ToString();
                button1.Text = "Update";
                button2.Text = "Refresh";

                designationAdd.Text = r.Cells[1].Value.ToString();
                textBox1.Text = r.Cells[2].Value.ToString();
                textBox2.Text = r.Cells[3].Value.ToString();
                dateTimePicker1.Value = Convert.ToDateTime(r.Cells[4].Value);
                //5 - Age
                textBox4.Text = r.Cells[6].Value.ToString();
                textBox5.Text = r.Cells[7].Value.ToString();
                checkBox1.Checked = System.Convert.ToBoolean(r.Cells[8].Value);
                textBox6.Text = r.Cells[8].Value.ToString();
            }
        }
    }
}