using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BookLibraryV1
{
    public partial class ManageGenres : Form
    {
        GenreTableAccessor genreTableAccessor;
        public ManageGenres(GenreTableAccessor gTA)
        {
            genreTableAccessor = gTA;
            InitializeComponent();
            populateList(genreTableAccessor.getGenres());
            AddNewGenre.Enabled = false;
            EditGenre.Enabled = false;
        }

        private void ManageGenres_Load(object sender, EventArgs e)
        {

        }

        private void populateList(List<String> genres)
        {
            foreach(String genre in genres)
            {
                GenreList.Items.Add(genre);
            }
        }

        private void SearchBtn_Click(object sender, EventArgs e)
        {
            GenreList.Items.Clear();
            if (SearchGenreTextBox.Text == "")
            {
                populateList(genreTableAccessor.getGenres());
                return;
            }
            populateList(genreTableAccessor.searchGenre(SearchGenreTextBox.Text.Trim()));
        }

        private void EditGenre_Click(object sender, EventArgs e)
        {
            try
            {
                genreTableAccessor.editGenre(GenreList.SelectedItems[0].SubItems[0].Text, newGenreTextBox.Text);
                GenreList.SelectedItems[0].SubItems[0].Text = newGenreTextBox.Text;
            }
            catch
            { }
        }
        private void AddNewGenre_Click(object sender, EventArgs e)
        {
            genreTableAccessor.addCustomGenre(newGenreTextBox.Text.Trim());
            GenreList.Items.Clear ();
            populateList(genreTableAccessor.getGenres());
        }

        private void newGenreTextBox_TextChanged(object sender, EventArgs e)
        {
            AddNewGenre.Enabled = !string.IsNullOrWhiteSpace(newGenreTextBox.Text);
            EditGenre.Enabled = !string.IsNullOrWhiteSpace(newGenreTextBox.Text);
        }

        private void DeleteGenre_Click(object sender, EventArgs e)
        {
            try 
            {
                genreTableAccessor.deleteGenre(GenreList.SelectedItems[0].SubItems[0].Text);
                GenreList.Items.Clear();
                populateList(genreTableAccessor.getGenres());
            }
            catch { }
        }
    }
}
