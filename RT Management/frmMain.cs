﻿using System;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace RT_Management
{
    public partial class frmMain : Form
    {
        private string user;
        private int level;

        public frmMain(string usuario, int nivel)
        {
            InitializeComponent();
            this.user = usuario;
            this.level = nivel;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            string estado = "Usuario activo: " + this.user;
            lblUser.Text = estado;
            if (this.level == 0 || this.level == 1)
            {
                menuAdmin.Visible = true;
            }
            else
            {
                menuAdmin.Visible = false;
            }

            if (this.level == 0)
            {
                deduciblesToolStripMenuItem1.Visible = true;
                deduciblesToolStripMenuItem1.Visible = true;
            }
            else 
            {
                deduciblesToolStripMenuItem1.Visible = false;
                deduciblesToolStripMenuItem1.Visible = false;
            }

            frmDeducibles.DefInstance.MdiParent = this;
            frmDeducibles.UsuarioActivo = this.user;
            frmDeducibles.levelUser = this.level;
            frmDeducibles.DefInstance.Activate();
            frmDeducibles.DefInstance.Show();
            fixMenu();
            this.Text = "RT Management v" + Assembly.GetExecutingAssembly().GetName().Version.ToString();

        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void deduciblesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmDeducibles.DefInstance.MdiParent = this;
            frmDeducibles.UsuarioActivo = this.user;
            frmDeducibles.levelUser = this.level;
            frmDeducibles.DefInstance.Activate();
            frmDeducibles.DefInstance.Show();
            fixMenu();
        }

        private void telefonosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Módulo Teléfonos", "Módulo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cambiarContraseñaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPasswordCambio pass = new frmPasswordCambio(this.user);
            pass.ShowDialog();
        }

        private void usuariosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmUsuarios.DefInstance.MdiParent = this;
            frmUsuarios.DefInstance.Activate();
            frmUsuarios.UsuarioActivo = user;
            frmUsuarios.DefInstance.Show();
            fixMenu();
        }

        private void enCascadaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void apiladasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void enParaleloToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void acercaDeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout about = new frmAbout();
            about.ShowDialog();
        }

        public string getUsuario()
        {
            return this.user;
        }

        private void deduciblesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmAdminDeducibles.DefInstance.MdiParent = this;
            frmAdminDeducibles.DefInstance.Show();
            frmAdminDeducibles.DefInstance.Activate();
            fixMenu();
        }

        private void verModificacionesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAdminModificaciones.DefInstance.MdiParent = this;
            frmAdminModificaciones.DefInstance.Activate();
            frmAdminModificaciones.DefInstance.Show();
            fixMenu();
        }

        private void fixMenu()
        {
            this.menu.Visible = false;
            this.menu.Visible = true;
            this.menu.ResumeLayout();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            frmInstalaciones.DefInstance.MdiParent = this;
            frmInstalaciones.DefInstance.Activate();
            frmInstalaciones.levelUser = this.level;
            frmInstalaciones.DefInstance.Show();
            fixMenu();
        }

        private void estadísticasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmStats stats = new frmStats();
            stats.ShowDialog();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            conexionBD bd = new conexionBD();
            bd.Conectar();
            bd.Modificar("UPDATE login SET active=0 WHERE user='" + user + "';");
            bd.Desconectar();
            Application.Exit();
        }

        private void pendientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPendientes.DefInstance.MdiParent = this;
            frmPendientes.UsuarioActivo = this.user;
            frmPendientes.DefInstance.Activate();            
            frmPendientes.DefInstance.Show();
            fixMenu();
        }

        public void backup(string tablas, string tipo)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo(@"C:\xampp\mysql\bin\mysqldump", tablas);

                string directorio = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                string nombreArchivo = DateTime.Now.ToString("yyyy-MM-dd HH_mm_ss");
                string filename = "";

                if (tipo == "Instalaciones")
                {
                    filename = directorio + "\\Dropbox\\Backup\\Instalaciones\\" + tipo + " " + nombreArchivo + ".sql";
                }
                else
                {
                    filename = directorio + "\\Dropbox\\Backup\\data " + nombreArchivo + ".sql";
                }
                //string filename = Path.Combine(@"C:\Documents and Settings\Josue\Escritorio\", "RESPALDO_PRUEBA_TABLAS" + DateTime.Now.ToString(@"yyyy-MM-dd.\sql"));

                using (StreamWriter writer = new StreamWriter(filename, false, Encoding.UTF8))
                {
                    using (Process process = new Process())
                    {
                        //psi.FileName = @"C:\Archivos de programa\bin\mysqldump";

                        psi.CreateNoWindow = true; // Evita que el proceso se inicie en una nueva ventana.
                        psi.UseShellExecute = false; // Evita que se use el shell del sistema operativo para iniciar el proceso.
                        psi.RedirectStandardOutput = true; // Escribir la salida en Process.StandarOuput
                        psi.StandardOutputEncoding = Encoding.UTF8; // Codificación de los datos de salida
                        process.StartInfo = psi;

                        process.OutputDataReceived +=

                        delegate (object sender, DataReceivedEventArgs e)
                        {
                            writer.WriteLine(e.Data);
                        };

                        process.Start();
                        process.BeginOutputReadLine(); // Lectura asincrónica del stream de salida
                        process.WaitForExit(); // Esperar a que el proceso termine.
                    }
                }
                MessageBox.Show("Copia de seguridad realizada con exito", "Respaldo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Se ha producido un error al realizar la copia de seguridad: " + exc, "Respaldo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        } //Backup

        private void menuBackupInfo_Click(object sender, EventArgs e)
        {
            backup("-u jsaucedo -p\"#4Hd&F&&y2K*yf%\" rtmanagement --add-drop-table --complete-insert --ignore-table=rtmanagement.instalaciones", "Información");
        }

        private void menuBackupInstalaciones_Click(object sender, EventArgs e)
        {
            backup("-u jsaucedo -p\"#4Hd&F&&y2K*yf%\" rtmanagement --add-drop-table --complete-insert --extended-insert --tables instalaciones", "Instalaciones");
        }

        private void correccionesSolicitadasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCorreccionesGpoInt.DefInstance.MdiParent = this;
            frmCorreccionesGpoInt.UsuarioActivo = this.user;
            frmCorreccionesGpoInt.levelUser = this.level;
            frmCorreccionesGpoInt.DefInstance.Activate();
            frmCorreccionesGpoInt.DefInstance.Show();
            fixMenu();
        }
    } //Clase
} //Namespace