using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Praktikum14
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        MySqlConnection sqlConnect = new MySqlConnection("server=localhost;uid=root;pwd=;database=premier_league");
        MySqlCommand sqlCommand;
        MySqlDataAdapter sqlAdapter;
        String sqlQuery;
        DataTable dtTeam = new DataTable();
        DataTable dtTopScorer = new DataTable();
        DataTable dtWorstDisipline = new DataTable();
        int PosisiSekarang = 0;
        string teamidsekarang;

        private void Form1_Load(object sender, EventArgs e)
        {
            sqlQuery = "SELECT t.team_name as 'Team Name', concat(m.manager_name, ' (' ,n.nation, ')') as 'Manager', concat(t.home_stadium, ', ',t.city,' (', t.capacity, ')') as 'Stadium', t.team_id FROM nationality n, dmatch d, manager m, team t, player p WHERE n.nationality_id = m.nationality_id and t.manager_id = m.manager_id and p.player_id = t.captain_id and d.player_id = p.player_id and d.team_id = t.team_id group by 1;";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtTeam);
            IsiDataPemain(0);
        }
        public void IsiDataPemain(int Posisi)
        {
            label_teamname.Text = dtTeam.Rows[Posisi][0].ToString();
            label_manager.Text = dtTeam.Rows[Posisi][1].ToString();
            label_stadium.Text = dtTeam.Rows[Posisi][2].ToString();         
            PosisiSekarang = Posisi;
            teamidsekarang = dtTeam.Rows[Posisi][3].ToString();
            DataTable dtPlayer = new DataTable();
            sqlQuery = "SELECT m.match_date , 'HOME' , t.team_name as `Lawan`, concat(m.goal_home, ' - ', m.goal_away) as 'SCORE'FROM `match` m , team t WHERE m.team_away = t.team_id AND m.team_home = '" + teamidsekarang + "'union SELECT m.match_date , 'AWAY' , t.team_name as `Lawan` , concat(m.goal_home, ' - ', m.goal_away) as 'SCORE'FROM `match` m , team t WHERE m.team_home = t.team_id AND m.team_away = '" + teamidsekarang + "' order by 1 desc limit 5;";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtPlayer);
            dgvPlayer.DataSource = dtPlayer;

            dtTopScorer = new DataTable();
            sqlQuery = "select concat(p.player_name, ' ', sum(if(d.type = 'GO',1,0)) + sum(if(d.type = 'GP',1,0)), '(', SUM(if(d.type = 'GP',1,0)), ')') as 'Top Scorer',sum(if(d.type = 'GO',1,0)) + sum(if(d.type = 'GP',1,0)) from player p, dmatch d where p.player_id = d.player_id and p.team_id = '" + teamidsekarang + "' group by p.player_id order by 2 desc;";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtTopScorer);
            label_topscorer.Text = dtTopScorer.Rows[0][0].ToString();

            dtWorstDisipline = new DataTable();
            sqlQuery = "SELECT concat(p.player_name, ', ', concat(sum(if (d.type = 'CY', 1, 0)), ' Yellow Card and ', sum(if (d.type = 'CR', 1, 0)), ' Red Card')) as 'Worst Disipline', sum(if (d.type = 'CY', 1,0)) +sum(if (d.type = 'CR', 1, 0)) from player p, dmatch d where p.player_id = d.player_id and p.team_id = '"+teamidsekarang+"' group by p.player_id order by 2 desc; ";
            sqlCommand = new MySqlCommand(sqlQuery, sqlConnect);
            sqlAdapter = new MySqlDataAdapter(sqlCommand);
            sqlAdapter.Fill(dtWorstDisipline);
            label_worstdisipline.Text = dtWorstDisipline.Rows[Posisi][0].ToString();
        }

        private void btn_first_Click(object sender, EventArgs e)
        {
            IsiDataPemain(0);
        }

        private void btn_prev_Click(object sender, EventArgs e)
        {
            if (PosisiSekarang > 0)
            {
                PosisiSekarang--;
                IsiDataPemain(PosisiSekarang);
            }
            else
            {
                MessageBox.Show("Data Sudah Data Pertama");
            }
        }

        private void btn_next_Click(object sender, EventArgs e)
        {
            if (PosisiSekarang < dtTeam.Rows.Count - 1)
            {
                PosisiSekarang++;
                IsiDataPemain(PosisiSekarang);
            }
            else
            {
                MessageBox.Show("Data Sudah Data Terakhir");
            }
        }

        private void btn_last_Click(object sender, EventArgs e)
        {
            IsiDataPemain(dtTeam.Rows.Count - 1);
        }
    }
}
