﻿using LibrarieModele;
using LibrarieModele.Enumerari;
using NivelStocareDate;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace EvidentaStudenti_UI_WindowsForms
{
    public partial class Form1 : Form
    {
        IStocareData adminStudenti;

        private Label lblAntetNume;
        private Label lblAntetPrenume;
        private Label lblAntetNote;

        private Label[] lblsNume;
        private Label[] lblsPrenume;
        private Label[] lblsNote;

        private const int LATIME_CONTROL = 100;
        private const int DIMENSIUNE_PAS_Y = 30;
        private const int DIMENSIUNE_PAS_X = 120;
        private const int OFFSET_X = 400;

        ArrayList disciplineSelectate = new ArrayList();

        public Form1()
        {
            InitializeComponent();
            adminStudenti = StocareFactory.GetAdministratorStocare();

            //setare proprietati
            this.Size = new Size(900, 400);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(100, 100);
            this.Font = new Font("Arial", 9, FontStyle.Bold);
            this.ForeColor = Color.LimeGreen;
            this.Text = "Informatii studenti";

            //adaugare control de tip Label pentru 'Nume';
            lblAntetNume = new Label();
            lblAntetNume.Width = LATIME_CONTROL;
            lblAntetNume.Text = "Nume";
            lblAntetNume.Left = OFFSET_X + DIMENSIUNE_PAS_X;
            lblAntetNume.ForeColor = Color.DarkGreen;
            this.Controls.Add(lblAntetNume);

            //adaugare control de tip Label pentru 'Prenume';
            lblAntetPrenume = new Label();
            lblAntetPrenume.Width = LATIME_CONTROL;
            lblAntetPrenume.Text = "Prenume";
            lblAntetPrenume.Left = OFFSET_X + 2 * DIMENSIUNE_PAS_X;
            lblAntetPrenume.ForeColor = Color.DarkGreen;
            this.Controls.Add(lblAntetPrenume);

            //adaugare control de tip Label pentru 'Note';
            lblAntetNote = new Label();
            lblAntetNote.Width = LATIME_CONTROL;
            lblAntetNote.Text = "Note";
            lblAntetNote.Left = OFFSET_X + 3 * DIMENSIUNE_PAS_X;
            lblAntetNote.ForeColor = Color.DarkGreen;
            this.Controls.Add(lblAntetNote);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AfiseazaStudenti();
        }

        private void AfiseazaStudenti()
        {
            List<Student> studenti = adminStudenti.GetStudenti();
            int nrStudenti = studenti.Count;

            lblsNume = new Label[nrStudenti];
            lblsPrenume = new Label[nrStudenti];
            lblsNote = new Label[nrStudenti];

            int i = 0;
            foreach (Student student in studenti)
            {
                //adaugare control de tip Label pentru numele studentilor;
                lblsNume[i] = new Label();
                lblsNume[i].Width = LATIME_CONTROL;
                lblsNume[i].Text = student.Nume;
                lblsNume[i].Left = OFFSET_X + DIMENSIUNE_PAS_X;
                lblsNume[i].Top = (i + 1) * DIMENSIUNE_PAS_Y;
                this.Controls.Add(lblsNume[i]);

                //adaugare control de tip Label pentru prenumele studentilor
                lblsPrenume[i] = new Label();
                lblsPrenume[i].Width = LATIME_CONTROL;
                lblsPrenume[i].Text = student.Prenume;
                lblsPrenume[i].Left = OFFSET_X + 2 * DIMENSIUNE_PAS_X;
                lblsPrenume[i].Top = (i + 1) * DIMENSIUNE_PAS_Y;
                this.Controls.Add(lblsPrenume[i]);

                //adaugare control de tip Label pentru notele studentilor
                lblsNote[i] = new Label();
                lblsNote[i].Width = LATIME_CONTROL;
                lblsNote[i].Text = string.Join(" ", student.GetNote());
                lblsNote[i].Left = OFFSET_X + 3 * DIMENSIUNE_PAS_X;
                lblsNote[i].Top = (i + 1) * DIMENSIUNE_PAS_Y;
                this.Controls.Add(lblsNote[i]);
                i++;
            }
        }

        private void btnAdauga_Click(object sender, EventArgs e)
        {
            lblNume.ForeColor = Color.LimeGreen;
            lblPrenume.ForeColor = Color.LimeGreen;

            if (!DateIntrareValide())
            {
                lblNume.ForeColor = Color.Red;
                lblPrenume.ForeColor = Color.Red;

                lblMesajEroare.Text = "Studentul exista deja";

                return;
            }

            Student s = new Student(0, txtNume.Text, txtPrenume.Text);
            s.SetNote(txtNote.Text);

            //set program studiu
            //verificare radioButton selectat
            ProgramStudiu specializareSelectata = GetProgramStudiuSelectat();
            s.Specializare = specializareSelectata;

            //set Discipline
            s.Discipline = new ArrayList();
            s.Discipline.AddRange(disciplineSelectate);

            adminStudenti.AddStudent(s);
            lblMesajEroare.Text = "";

            //resetarea controalelor pentru a introduce datele unui student nou
            ResetareControale();
        }

        private bool DateIntrareValide()
        {
            string nume = txtNume.Text;
            string prenume = txtPrenume.Text;

            Student studentCuAcelasiNume = adminStudenti.GetStudent(nume, prenume);

            return studentCuAcelasiNume == null;
        }

        private void CkbDiscipline_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox checkBoxControl = sender as CheckBox; //operator 'as'
            //sau
            //CheckBox checkBoxControl = (CheckBox)sender;  //operator cast

            string disciplinaSelectata = checkBoxControl.Text;

            //verificare daca checkbox-ul asupra caruia s-a actionat este selectat
            if (checkBoxControl.Checked == true)
                disciplineSelectate.Add(disciplinaSelectata);
            else
                disciplineSelectate.Remove(disciplinaSelectata);
        }

        private ProgramStudiu GetProgramStudiuSelectat()
        {
            if (rdbCalculatoare.Checked)
                return ProgramStudiu.Calculatoare;
            if (rdbAutomatica.Checked)
                return ProgramStudiu.Automatica;
            if (rdbElectronica.Checked)
                return ProgramStudiu.Electronica;

            return ProgramStudiu.Calculatoare;
        }

        private void btnAfiseaza_Click(object sender, EventArgs e)
        {
            List<Student> studenti = adminStudenti.GetStudenti();
            AfiseazaStudenti();
            AfisareStudentiInControlListbox(studenti);
            AfisareStudentiInControlDataGridView(studenti);
        }

        private void AfisareStudentiInControlListbox(List<Student> studenti)
        {
            lstAfisare.Items.Clear();
            foreach (Student student in studenti)
            {
                //pentru a adauga un obiect de tip Student in colectia de Items a unui control de tip ListBox, 
                // clasa Student trebuie sa implementeze metoda ToString() specificand cuvantul cheie 'override' in definitie
                //pentru a arata ca metoda ToString a clasei de baza (Object) este suprascrisa
                lstAfisare.Items.Add(student);

                //personalizare sursa de date
                //lstAfisare.Items.Add(s.NumeComplet);
            }
        }

        private void AfisareStudentiInControlDataGridView(List<Student> studenti)
        {
            //reset continut control DataGridView
            dataGridStudenti.DataSource = null;

            //!!!! Controlul de tip DataGridView are ca sursa de date lista de obiecte de tip Student !!!
            dataGridStudenti.DataSource = studenti;

            // personalizare sursa de date
            // dataGridStudenti.DataSource = studenti.Select(s => new { s.Id, s.Nume, s.Prenume, 
            // s.Specializare, Discipline = string.Join(",", s.Discipline), 
            // Note = string.Join(",", s.GetNote()) } ).ToList();
        }

        private void ResetareControale()
        {
            txtNume.Text = txtPrenume.Text = txtNote.Text = string.Empty;

            rdbCalculatoare.Checked = false;
            rdbAutomatica.Checked = false;
            rdbElectronica.Checked = false;

            ckbPCLP.Checked = false;
            ckbPOO.Checked = false;
            ckbPIU.Checked = false;

            disciplineSelectate.Clear();
            lblMesajEroare.Text = string.Empty;
        }
    }
}
