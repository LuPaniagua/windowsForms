using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace crud
{
    public partial class frmCadastrodeClientes : Form
    {

        //Conexão com o banco de dados MySQL
        MySqlConnection Conexao;
        string data_source = "datasource = localhost; username=root; password=; database=db_cadastro";

        public frmCadastrodeClientes()
        {
            InitializeComponent();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                //Validação de campos obrigatórios
                if (string.IsNullOrEmpty(txtNomeCompleto.Text.Trim()) ||
                    string.IsNullOrEmpty(txtEmail.Text.Trim()) ||
                    string.IsNullOrEmpty(txtCPF.Text.Trim()))
                {
                    MessageBox.Show("Todos os campos devem ser preenchidos.",
                        "Validação",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return; //Impede o prosseguimento se algum campo estiver vazio
                }

                //Validação do CPF
                string cpf = txtCPF.Text.Trim();

                if (!isValidCPFLength(cpf))
                {
                    MessageBox.Show("CPF inválido.Certifique-se de que o CPF tenha 11 dígitos numéricos.",
                        "Validação",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return; //Impede o prosseguimento se o CPF for inválido
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

            //Função para validar o comprimento e formato do CPF

            private bool isValidCPFLength(string cpf)
            {
                //Remove todos os caracteres não numéricos
                cpf = new string(cpf.Where(char.IsDigit).ToArray());

                //Verifica se o CPF tem exatamente 11 dígitos
                return cpf.Length == 11;
            }

        }
    }