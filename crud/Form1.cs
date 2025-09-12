﻿using System;
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

        private int ?codigo_cliente = null;

        public frmCadastrodeClientes()
        {
            InitializeComponent();

            //Configuração inicial do ListView para exibição dos dados dos clientes
            lstCliente.View = View.Details;             //Define a visualização em "detalhes"
            lstCliente.LabelEdit = true;                //Permite editar os títulos das colunas
            lstCliente.AllowColumnReorder = true;       //Permite reordenar as colunas
            lstCliente.FullRowSelect = true;            // Seleciona a linha inteira ao clicar
            lstCliente.GridLines = true;                //Exibe as linhas de grande no ListView

            //Definindo as colunas as ListView
            lstCliente.Columns.Add("Codigo", 100, HorizontalAlignment.Left);       //Coluna de Código
            lstCliente.Columns.Add("Nome Completo",200, HorizontalAlignment.Left); //Coluna de Nome completo
            lstCliente.Columns.Add("Nome Social", 200, HorizontalAlignment.Left);  //Coluna de Nome Social
            lstCliente.Columns.Add("E-mail", 200, HorizontalAlignment.Left);       //Coluna de E-mail
            lstCliente.Columns.Add("CPF", 200, HorizontalAlignment.Left);          //Coluna de CPF

            //Carrega os dados dos clientes na interface
            carregar_cliente();
        }


        private void carregar_clientes_com_query(string query)
        {
            try
            {

                //Cria a conexão com o banco de dados
                Conexao = new MySqlConnection(data_source);
                Conexao.Open();

                //Executa a consulta SQL fornecida
                MySqlCommand cmd = new MySqlCommand(query, Conexao);

                //Se a consulta contém o parâmetro @q, adiciona o valor da caixa de pesquisa
                if (query.Contains("@q"))
                {
                    cmd.Parameters.AddWithValue("@q", "%" + txtBuscar.Text + "%");
                }

                //Executa o comando e obtém os resultaados
                MySqlDataReader reader = cmd.ExecuteReader();

                //Limpa os itens existentes no ListView antes de adicionar novos
                lstCliente.Items.Clear();

                //Preencha o ListView com os dados do cliente
                while (reader.Read())
                {
                    //Cria uma linha para cada cliente com os dedos retornados da consulta
                    string[] row =
                    {
                        Convert.ToString(reader.GetInt32(0)), //Código
                        reader.GetString(1),                  //Nome Completo
                        reader.GetString(2),                  //Nome Social
                        reader.GetString(3),                  //E-mail
                        reader.GetString(4),                  //CPF
                    };
                    //Adiciona a linha ao ListView
                    lstCliente.Items.Add(new ListViewItem(row));

                };


            }
            catch (MySqlException ex)
            {
                //Tratar erros relacionados ao MYSQL
                MessageBox.Show("Erro" + ex.Number + " ocorreu: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception ex)
            {
                //Tratar outros tipos de erro
                MessageBox.Show(" Ocorreu: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            finally
            {
                //Garante que a coneão com o banco será fechada, mesmo se ocorrer erro
                if (Conexao != null && Conexao.State == ConnectionState.Open)
                {
                    Conexao.Close();
                }
            }
        }


        //Método para carregar todos os clientes no ListView (usando uma consula sem parâmetros)
        private void carregar_cliente()
        {
            string query = "SELECT * FROM dadosdocliente ORDER BY idcliente DESC";
            carregar_clientes_com_query(query);
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                //Validação do CPF
                string cpf = txtCPF.Text.Trim();


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

                //Cria a conexão com o banco de dados
                Conexao = new MySqlConnection(data_source);
                Conexao.Open();

                //Comando SQL para inserir um novo cliente no banco de dados
                MySqlCommand cmd = new MySqlCommand
                {
                    Connection = Conexao,
                };

                cmd.Prepare();

                if (codigo_cliente == null)
                {
                    //Insert

                    cmd.CommandText = "INSERT INTO dadosdocliente (nomeCompleto, nomeSocial, email, cpf)" +
                    "VALUES(@nomecompleto, @nomesocial, @email, @cpf)";

                    //Adiciona os parâmetros com os dados do formulário
                    cmd.Parameters.AddWithValue("@nomecompleto", txtNomeCompleto.Text.Trim());
                    cmd.Parameters.AddWithValue("@nomesocial", txtNomeSocial.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@cpf", cpf);

                    //Executa o comando de inserção no banco
                    cmd.ExecuteNonQuery();

                    //Mensagem de sucesso
                    MessageBox.Show("Contato inserido com Sucesso: ",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                }
                else
                {
                    //Update
                    cmd.CommandText = $"UPDATE `dadosdocliente` SET " +
                    $"nomecompleto = @nomecompleto, " +
                    $"nomesocial = @nomesocial, " +
                    $"email = @email, " +
                    $"cpf = @cpf " +
                    $"WHERE idcliente = @codigo ";

                    cmd.Parameters.AddWithValue("@nomecompleto", txtNomeCompleto.Text.Trim());
                    cmd.Parameters.AddWithValue("@nomesocial", txtNomeSocial.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@cpf", cpf);
                    cmd.Parameters.AddWithValue("@codigo", codigo_cliente);

                    //Executa o comando de alteração no banco
                    cmd.ExecuteNonQuery ();

                    //Mensagem de sucesso para dados atualizados
                    MessageBox.Show($"Os dados com o código {codigo_cliente} foram alterados com sucesso!",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                }

                codigo_cliente = null;

                //Limpa os campos após o sucesso
                txtNomeCompleto.Text = String.Empty;
                txtNomeSocial.Text = " ";
                txtEmail.Text = " ";
                txtCPF.Text = " ";

                //Recarrega os clientes no ListView
                carregar_cliente();



                //Muda para a aba de pesquisa
                tbControl.SelectedIndex = 1;


                if (!isValidCPFLength(cpf))
                {
                    MessageBox.Show("CPF inválido.Certifique-se de que o CPF tenha 11 dígitos numéricos.",
                        "Validação",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return; //Impede o prosseguimento se o CPF for inválido
                }
            }
            catch (MySqlException ex)
            {
                //Tratar erros relacionados ao MYSQL
                MessageBox.Show("Erro" + ex.Number + " ocorreu: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception ex)
            {
                //Tratar outros tipos de erro
                MessageBox.Show(" Ocorreu: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            finally
            { 
            //Garante que a coneão com o banco será fechada, mesmo se ocorrer erro
            if (Conexao != null && Conexao.State == ConnectionState.Open)
                   {
                    Conexao.Close();
                }

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

        private void btnPesquisar_Click(object sender, EventArgs e)
        {
            string query = "SELECT * FROM dadosdocliente WHERE nomecompleto LIKE @q OR nomesocial LIKE @q ORDER BY idcliente DESC";
            carregar_clientes_com_query(query);
        }

        private void lstCliente_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            ListView.SelectedListViewItemCollection clientedaselecao = lstCliente.SelectedItems;

            foreach (ListViewItem item in clientedaselecao)
            {
                codigo_cliente = Convert.ToInt32(item.SubItems[0].Text);

                //Exibe uma MenssageBox com o código do cliente selecionado
                MessageBox.Show("Código do Cliente: " + codigo_cliente.ToString(),
                    "Código Selecionado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                txtNomeCompleto.Text = item.SubItems[1].Text;
                txtNomeSocial.Text = item.SubItems[2].Text;
                txtEmail.Text = item.SubItems[3].Text;
                txtCPF.Text = item.SubItems[4].Text;
            }
        }

        private void btnNovoCliente_Click(object sender, EventArgs e)
        {
            codigo_cliente = null;

            txtNomeCompleto.Text = String.Empty;
            txtNomeSocial.Text = " ";
            txtEmail.Text = " ";
            txtCPF.Text = " ";

            txtNomeCompleto.Focus();
        }
    }
}