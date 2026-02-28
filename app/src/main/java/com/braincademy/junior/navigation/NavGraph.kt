package com.braincademy.junior.navigation

import androidx.compose.runtime.Composable
import androidx.lifecycle.viewmodel.compose.viewModel
import androidx.navigation.compose.NavHost
import androidx.navigation.compose.composable
import androidx.navigation.compose.rememberNavController
import com.braincademy.junior.screens.DifficultyScreen
import com.braincademy.junior.screens.GameScreen
import com.braincademy.junior.screens.MenuScreen
import com.braincademy.junior.screens.ResultsScreen
import com.braincademy.junior.screens.SnakeSpellResultsScreen
import com.braincademy.junior.screens.SnakeSpellScreen
import com.braincademy.junior.viewmodel.GameViewModel
import com.braincademy.junior.viewmodel.SnakeSpellViewModel

object Routes {
    const val MENU = "menu"
    const val DIFFICULTY = "difficulty"
    const val GAME = "game"
    const val RESULTS = "results"
    const val SNAKE_DIFFICULTY = "snake_difficulty"
    const val SNAKE_SPELL = "snake_spell"
    const val SNAKE_RESULTS = "snake_results"
}

@Composable
fun AppNavGraph(
    quizViewModel: GameViewModel = viewModel(),
    snakeViewModel: SnakeSpellViewModel = viewModel(),
) {
    val navController = rememberNavController()

    NavHost(navController = navController, startDestination = Routes.MENU) {
        // ── Quiz Flow ──
        composable(Routes.MENU) {
            MenuScreen(
                stats = quizViewModel.stats,
                onCategorySelected = { category ->
                    quizViewModel.selectCategory(category)
                    navController.navigate(Routes.DIFFICULTY)
                },
                onSnakeSpellSelected = {
                    navController.navigate(Routes.SNAKE_DIFFICULTY)
                },
            )
        }
        composable(Routes.DIFFICULTY) {
            DifficultyScreen(
                category = quizViewModel.category,
                onDifficultySelected = { difficulty ->
                    quizViewModel.startGame(difficulty)
                    navController.navigate(Routes.GAME) {
                        popUpTo(Routes.MENU)
                    }
                },
                onBack = { navController.popBackStack() },
            )
        }
        composable(Routes.GAME) {
            GameScreen(
                viewModel = quizViewModel,
                onGameOver = {
                    navController.navigate(Routes.RESULTS) {
                        popUpTo(Routes.MENU)
                    }
                },
            )
        }
        composable(Routes.RESULTS) {
            ResultsScreen(
                score = quizViewModel.gameScore,
                correctCount = quizViewModel.correctCount,
                totalQuestions = GameViewModel.TOTAL_QUESTIONS,
                accuracy = quizViewModel.accuracy,
                starCount = quizViewModel.starCount,
                onPlayAgain = {
                    quizViewModel.startGame(quizViewModel.difficulty)
                    navController.navigate(Routes.GAME) {
                        popUpTo(Routes.MENU)
                    }
                },
                onMenu = {
                    navController.navigate(Routes.MENU) {
                        popUpTo(Routes.MENU) { inclusive = true }
                    }
                },
            )
        }

        // ── Snake Spellcaster Flow ──
        composable(Routes.SNAKE_DIFFICULTY) {
            DifficultyScreen(
                title = "Snake Spellcaster \u2014 Select Difficulty",
                onDifficultySelected = { difficulty ->
                    snakeViewModel.startGame(difficulty)
                    navController.navigate(Routes.SNAKE_SPELL) {
                        popUpTo(Routes.MENU)
                    }
                },
                onBack = { navController.popBackStack() },
            )
        }
        composable(Routes.SNAKE_SPELL) {
            SnakeSpellScreen(
                viewModel = snakeViewModel,
                onGameOver = {
                    navController.navigate(Routes.SNAKE_RESULTS) {
                        popUpTo(Routes.MENU)
                    }
                },
            )
        }
        composable(Routes.SNAKE_RESULTS) {
            SnakeSpellResultsScreen(
                score = snakeViewModel.battlefield.score,
                wavesCompleted = snakeViewModel.battlefield.currentWave - 1,
                snakesKilled = snakeViewModel.battlefield.snakesKilled,
                questionsAnswered = snakeViewModel.battlefield.questionsAnswered,
                questionsCorrect = snakeViewModel.battlefield.questionsCorrect,
                onPlayAgain = {
                    snakeViewModel.startGame(snakeViewModel.difficulty)
                    navController.navigate(Routes.SNAKE_SPELL) {
                        popUpTo(Routes.MENU)
                    }
                },
                onMenu = {
                    navController.navigate(Routes.MENU) {
                        popUpTo(Routes.MENU) { inclusive = true }
                    }
                },
            )
        }
    }
}
