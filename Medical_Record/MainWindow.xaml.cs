﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Medical_Record
{
    public class MedicalContext : DbContext
    {
        public DbSet<Patient> Pacientes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=patients.db");
        }
    }

    public class Patient
    {
        public int Id { get; set; }
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public int? Idade { get; set; }
        public string? Telefone { get; set; }
        public string? Consultas { get; set; }
    }

    public partial class MainWindow : Window
    {
        private readonly MedicalContext _context;

        public MainWindow()
        {
            InitializeComponent();
            _context = new MedicalContext();
            _context.Database.Migrate();
        }

        private void OnMenuItemClicked(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (button != null)
            {
                switch (button.Content.ToString())
                {
                    case "Dashboard":
                        ShowDashboard();
                        break;
                    case "Agenda":
                        ShowAgenda();
                        break;
                    case "Pacientes":
                        ShowPacientes();
                        break;
                    case "Finanças":
                        ShowFinancas();
                        break;
                    case "Educação":
                        ShowEducacao();
                        break;
                    case "Suporte":
                        ShowSuporte();
                        break;
                    case "Configurações":
                        ShowConfiguracoes();
                        break;
                    case "Sair":
                        Application.Current.Shutdown();
                        break;
                }
            }
        }

        private void ShowDashboard() => MainContent.Content = new TextBlock { Text = "Dashboard", FontSize = 30 };
        private void ShowAgenda() => MainContent.Content = new TextBlock { Text = "Agenda", FontSize = 30 };
        private void ShowFinancas() => MainContent.Content = new TextBlock { Text = "Finanças", FontSize = 30 };
        private void ShowEducacao() => MainContent.Content = new TextBlock { Text = "Educação", FontSize = 30 };
        private void ShowSuporte() => MainContent.Content = new TextBlock { Text = "Suporte", FontSize = 30 };
        private void ShowConfiguracoes() => MainContent.Content = new TextBlock { Text = "Configurações", FontSize = 30 };

        private void ShowPacientes()
        {
            PacientesView.Visibility = Visibility.Visible;
            MainContent.Content = PacientesView;
            LoadPacientes();
        }

        private void LoadPacientes()
        {
            PacientesListView.ItemsSource = _context.Pacientes.ToList();
        }

        private void AddPaciente(string nome, string email, int idade, string telefone, string consultas)
        {
            var paciente = new Patient
            {
                Nome = nome,
                Email = email,
                Idade = idade,
                Telefone = telefone,
                Consultas = consultas
            };

            _context.Pacientes.Add(paciente);
            _context.SaveChanges();
            LoadPacientes();
        }

        private void OnAdicionarPacienteClicked(object sender, RoutedEventArgs e)
        {
            string nome = NomeTextBox.Text;
            string email = EmailTextBox.Text;
            int idade = int.TryParse(IdadeTextBox.Text, out int result) ? result : 0;
            string telefone = TelefoneTextBox.Text;
            string consultas = ConsultasTextBox.Text;

            if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(telefone))
            {
                MessageBox.Show("Nome e Telefone são campos obrigatórios.");
                return;
            }

            AddPaciente(nome, email, idade, telefone, consultas);
        }

        private void OnDeletePacienteClicked(object sender, RoutedEventArgs e)
        {
            var selectedPatient = PacientesListView.SelectedItem as Patient;

            if (selectedPatient != null)
            {
                MessageBoxResult result = MessageBox.Show($"Deseja realmente excluir o paciente {selectedPatient.Nome}?",
                                                          "Confirmar Exclusão", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    DeletePaciente(selectedPatient.Id);
                    LoadPacientes();
                }
            }
            else
            {
                MessageBox.Show("Selecione um paciente para excluir.");
            }
        }

        private void DeletePaciente(int patientId)
        {
            var patient = _context.Pacientes.Find(patientId);
            if (patient != null)
            {
                _context.Pacientes.Remove(patient);
                _context.SaveChanges();
            }
        }
    }
}