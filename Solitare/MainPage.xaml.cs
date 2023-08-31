//G00410308
//Everything that is part of the video search for: VIDEO
//EDIT: reduced padding to fit on all devices ^


namespace Solitare;

using System.Timers;
using Windows.Devices.PointOfService;

public partial class MainPage : ContentPage
{

    //Variables
    Timer timer = new Timer();
    List<List<IView>> marbles = new List<List<IView>>();
    List<int> highscores = new List<int>();

    int score = 0;
    int time = 0;

    int completion_bonus = 1000;

    int points_per_marble = 100;
    int points_per_bonus_marble = 50;

    int expected_time = 300;
    int expected_time_bonus = 500;

    int grid_size = 7;

    int selected_x = -1;
    int selected_y = -1;



    //Main
    public MainPage()
    {
        //initalize
        InitializeComponent();

        //show start menu
        Show_StartMenu();

        //set timer interval to 1 second and then connects to function
        timer.Interval = 1000;
        timer.Elapsed += TimerStep;


        //create the 2d list
        for (int i = 0; i < grid_size; i++)
        {
            List<IView> list = new List<IView>();
            marbles.Add(list);

        }
    }



    //Timer
    private void TimerStep(object sender, EventArgs e)
    {
        //increase the time
        time += 1;

        //dispatch to update timer
        Dispatcher.Dispatch(() =>
        {
            UpdateTimer();
        });
    }

    //Update the timer
    private void UpdateTimer()
    {
        //updates timer to show
        Timer.Text = "Time: " + time.ToString();
    }

    //Reset Timer
    private void ResetTimer()
    {
        //stop the timer and reset the time
        timer.Stop();
        time = 0;

        //updates timer
        UpdateTimer();
    }

    //Create the grid
    private void CreateGrid()
    {
        //create new grid
        Grid grid = new Grid();
        grid.HorizontalOptions = LayoutOptions.Center;
        grid.VerticalOptions = LayoutOptions.Center;
        grid.ColumnSpacing = 5;
        grid.RowSpacing = 5;

        //create rows and columns
        for (int i = 0; i < grid_size; i++)
        {
            RowDefinition row = new RowDefinition();
            ColumnDefinition column = new ColumnDefinition();

            grid.RowDefinitions.Add(row);
            grid.ColumnDefinitions.Add(column);
        }

        //make buttons and fill in grid
        for (int y = 0; y < grid_size; y++)
        {
            for (int x = 0; x < grid_size; x++)
            {
                //check if current cell is a corner / not button
                if ((y < 2 || y >= grid_size - 2) && (x < 2 || x >= grid_size - 2))
                {
                    //create box for unused cells
                    BoxView boxView = new BoxView();
                    boxView.HorizontalOptions = LayoutOptions.Center;
                    boxView.VerticalOptions = LayoutOptions.Center;
                    boxView.Color = Colors.Transparent;
                    boxView.WidthRequest= 20;
                    boxView.HeightRequest= 20;

                    //set x and y
                    Grid.SetRow(boxView, y);
                    Grid.SetColumn(boxView, x);

                    //add to grid and list
                    grid.Children.Add(boxView);
                    marbles[y].Add(boxView);
                }
                else
                {
                    //create button for playable cells
                    Button button = new Button();
                    button.HorizontalOptions = LayoutOptions.Center;
                    button.VerticalOptions = LayoutOptions.Center;
                    button.ImageSource = "marble";
                    button.BackgroundColor= Colors.LightBlue;
                    button.WidthRequest= 20;
                    button.HeightRequest= 20;
                    button.CornerRadius = 20 / 2;

                    if ((x == (grid_size - 1) / 2) && (y == (grid_size - 1) / 2))
                    {
                        button.Opacity = 0;
                    }

                    else
                    {
                     button.Opacity = 1;

                    }

                    //set x and y
                    Grid.SetRow(button, y);
                    Grid.SetColumn(button, x);


                    //add functions to the button
                    Int16 _x = Int16.Parse(x.ToString());
                    Int16 _y = Int16.Parse(y.ToString());
                    button.Clicked += new EventHandler((sender, e) =>
                        {
                            //pass x and y to button
                            Marble_Clicked(sender, e, _x, _y);
                        });

                    //add to grid and list
                    grid.Children.Add(button);
                    marbles[y].Add(button);
                }
            }
        }


        Board.Children.Add(grid);
    }



    //Destroys the grid
    private void DestroyGrid()
    {
        //remove all objects from the board
        foreach (IView grid in Board.Children.ToList())
        {
            Board.Children.Remove(grid);
        }

        //clear the marbles list
        foreach (List<IView> List in marbles)
        {
            List.Clear();
        }

    }



    //Make start menu visible
    private void Show_StartMenu()
    {
        StartMenu.IsVisible = true;
        GameMenu.IsVisible = false;
        EndMenu.IsVisible = false;
        HighScores.IsVisible = false;
        RulesMenu.IsVisible = false;
    }

    //Make game menu visible
    private void Show_GameMenu()
    {
        GameMenu.IsVisible = true;
        StartMenu.IsVisible = false;
        EndMenu.IsVisible = false;
        HighScores.IsVisible = false;
        RulesMenu.IsVisible = false; 
    }

    //Make end menu visible
    private void Show_EndMenu()
    {
        EndMenu.IsVisible = true;
        StartMenu.IsVisible = false;
        GameMenu.IsVisible = false;
        HighScores.IsVisible = false;
        RulesMenu.IsVisible = false;

    }

    //Make high scores visible
    private void Show_HighScores()
    {
        HighScores.IsVisible = true;
        StartMenu.IsVisible = false;
        GameMenu.IsVisible = false;
        EndMenu.IsVisible = false;
        RulesMenu.IsVisible = false;
    }

    //Make rules visible <VIDEO>
    private void Show_Rules()
    {
        RulesMenu.IsVisible = true;
        StartMenu.IsVisible = false;
        EndMenu.IsVisible = false;
        HighScores.IsVisible = false;
    }

    //Start game function
    private void StartGame()
    {

        //update the score
        score = 0;
        GameScore.Text = "Score: " + score.ToString();

        //reset and start timer
        ResetTimer();
        timer.Start();

        //reset and create grid
        DestroyGrid();
        CreateGrid();

        //Shows the game
        Show_GameMenu();
    }

    //End game function
    private void EndGame() // Avengers TM
    {
        //stop the timer
        timer.Stop();

        //get bonus if completed under 5 minutes
        if (time < expected_time)
        {
            score += expected_time_bonus;
        }

        if (CheckForLast())
        {
            score += completion_bonus;
        }

        // add to highscores
        AddHighScore(score);

        //updates the score to text
        EndScore.Text = "Score: " + score.ToString();


        //shows end menu
        Show_EndMenu();


    }

    // highscores
    private void AddHighScore(int score_to_add)
    {
        // add to list
        highscores.Add(score_to_add);

        UpdateHighScores();
    }

    private void UpdateHighScores()
    {
        // sort high scores
        highscores.Sort();

        //clear existing scores
        foreach (IView x in HighScoresList.Children.ToList())
        {
            HighScoresList.Children.Remove(x);
        }

        // add scores
        for(int current = highscores.Count - 1; current >= 0; current--)
        {
            if (current >= 0 && current < highscores.Count)
            {
                // create high score object
                HorizontalStackLayout scoreBox = new HorizontalStackLayout();
                scoreBox.HorizontalOptions = LayoutOptions.Center;
                scoreBox.VerticalOptions = LayoutOptions.Center;
                scoreBox.Spacing = 40;

                //label for positin
                Label positionLabel = new Label();
                positionLabel.HorizontalTextAlignment = TextAlignment.Start;
                positionLabel.Text = "#" + ((highscores.Count - (current + 1)) + 1).ToString();
                positionLabel.TextColor = Colors.Black;
                scoreBox.Children.Add(positionLabel);

                //createlabel for score
                Label scoreLabel = new Label();
                scoreLabel.HorizontalTextAlignment = TextAlignment.End;
                scoreLabel.Text = (highscores[current]).ToString();
                scoreLabel.TextColor = Colors.Black;
                scoreBox.Children.Add(scoreLabel);

                // add to ui
                HighScoresList.Children.Add(scoreBox);

            }

        }
    }

    //Marbles
    private bool CheckForLast()
    {
        for (int y = 0; y < grid_size; y++)
        {
            for (int x = 0; x < grid_size; x++)
            {
                bool check = CheckForMarble(y, x);
                if (y == (grid_size - 1)/2 && x == (grid_size - 1)/2)
                {
                    if (check == false)
                    {
                        return false;
                    }
                }
                else
                {
                    if (check == true)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private bool CheckForMarble(int y, int x)
    {
        //check if marble is there
        try
        {
            if (((Button) marbles[y][x]).Opacity > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch {
            return false;
        }
    }

    private void ClearMarbles(int start_y, int end_y, int start_x, int end_x)
    {

        if (start_x == end_x)
        {
            for (int y = start_y; y != end_y; y = y)
            {
                try
                {
                    //clear current marble
                    Button button = (Button)marbles[y][start_x];
                    button.Opacity = 0;
                }
                catch (Exception e)
                {

                }

                if (y > end_y)
                {
                    y--;
                }
                else
                {
                    y++;
                }
            }
        }
        else
        {
            for (int x = start_x; x != end_x; x = x)
            {
                try
                {
                    //clear current marble
                    Button button = (Button)marbles[start_y][x];
                    button.Opacity = 0;
                }
                catch (Exception e)
                {

                }

                if (x > end_x)
                {
                    x--;
                }
                else
                {
                    x++;
                }
            }
        }
    }

    private bool MoveMarble(int start_y, int start_x, int end_y, int end_x)
    {
        //check if any negative
        if(start_y < 0 || start_x < 0 || end_y < 0 || end_x < 0)
        { 
            return false; 
        }

        // check if space is 1
        int diffy = start_y- end_y;
        int diffx = start_x- end_x;
        if (diffy < 0)
        {
            diffy *= -1;
        }
        if (diffx < 0)
        {
            diffx*= -1;
        }
        if (diffx == 1 || diffy == 1)
        {
            return false;
        }

        //cehck if the start doesnt have a marble
        if (CheckForMarble(start_y, start_x) == false)
        {
            return false;
        }

        //cehck if end has a marble
        if (CheckForMarble(end_y, end_x) == true)
        {
            return false;
        }

        //check and count marbles
        int count = CountMarbles(start_y, start_x, end_y, end_x);
        if(count == -1)
        {
            return false;
        }

        //clear marbles
        ClearMarbles(start_y, end_y, start_x, end_x);

        //update moved marble
        Button button = (Button)marbles[end_y][end_x];
        button.Opacity = 1;

        //update score
        for(int i =0; i < count; i++)
        {
            score += points_per_marble;

            //add bonus score per marble
            if (i > 0)
            {
                score += points_per_bonus_marble;
            }

            GameScore.Text = "Score: " + score.ToString();
        }

        //unselectmarble
        Button last = (Button)marbles[selected_y][selected_x];
        last.Opacity = 0;
        selected_y = -1;
        selected_x= -1;

        //passed all checks
        return true;
    }

    private int CountMarbles(int start_y, int start_x, int end_y, int end_x)
    {
        //count variable
        int count = -1;

        //boolean for checking if it should have a marble
        bool should_have_marble = true;

        if (((start_y - end_y) % 2 == 1) || ((start_x - end_x) % 2 == 1))
        {
            return -1;
        }

        //check if on same line
        if (start_y == end_y)
        {
            //check x
            for (int x = start_x; x != end_x; x = x)
            {
                //marble variable
                bool has_marble = CheckForMarble(start_y, x);

                //check for marble
                if (has_marble != should_have_marble)
                {
                    //cell occupied
                    return -1;
                }

                //update marble count
                if(has_marble)
                {
                    count++;
                }

                //update if it should have a marble
                if(x != start_x)
                {
                    should_have_marble = !should_have_marble;
                }

                //update loop
                if(x > end_x)
                {
                    x--;
                }
                else
                {
                    x++;
                }
            }

            
        }

        else if (start_x == end_x)
        {
            //check y
            for (int y = start_y; y != end_y; y = y)
            {
                //marble variable
                bool has_marble = CheckForMarble(y, start_x);

                //check for marble
                if (has_marble != should_have_marble)
                {
                    //cell occupied
                    return -1;
                }

                //update marble count
                if (has_marble)
                {
                    count++;
                }

                //update if it should have a marble
                if (y != start_y)
                {
                    should_have_marble = !should_have_marble;
                }

                //update loop
                if (y > end_y)
                {
                    y--;
                }
                else
                {
                    y++;
                }
            }

        }

        //return the count
        return count;
    }

    private void SelectMarble(int y, int x)
    {
        if(CheckForMarble(y, x)== true)
        {
            //update old selection opacity
            if(selected_y >= 0 && selected_x >= 0)
            {

            Button old_button = (Button)marbles[selected_y][selected_x];
            old_button.Opacity = 1;

            }
            //select the marble
            selected_x = x;
            selected_y = y;

            //change the opacity
            Button button = (Button)marbles[y][x];
            button.Opacity= .5;
        }
    }

    //New Game Button
    private void NewGame_Clicked(object sender, EventArgs e)
    {
        StartGame();
    }
    //High scores Button 
    private void HighScores_Button(object sender, EventArgs e)
    {
        Show_HighScores();
    }
    //Exit Button
    private void Exit_Button(object sender, EventArgs e)
    {
        System.Diagnostics.Process.GetCurrentProcess().Kill();
    }
    //Back Button
    private void Back_Button(object sender, EventArgs e)
    {
        Show_StartMenu();
    }

    //Marble Button
    private void Marble_Clicked(object sender, EventArgs e, int x, int y)
    {
        //attempt to move the marbe
        bool moved = MoveMarble(selected_y, selected_x, y, x);

        if(moved == false)
        {
            SelectMarble(y, x);
        }


    }

    private void End_Button(System.Object sender, System.EventArgs e)
    {
        EndGame();
    }

    private void Rules_Button(System.Object sender, System.EventArgs e)
    {
        Show_Rules();
    }
}