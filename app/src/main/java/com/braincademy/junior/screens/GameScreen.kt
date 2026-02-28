package com.braincademy.junior.screens

import androidx.compose.animation.AnimatedVisibility
import androidx.compose.animation.fadeIn
import androidx.compose.animation.fadeOut
import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.aspectRatio
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.lazy.grid.GridCells
import androidx.compose.foundation.lazy.grid.LazyVerticalGrid
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.LinearProgressIndicator
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.braincademy.junior.model.GameQuestion
import com.braincademy.junior.ui.theme.Background
import com.braincademy.junior.ui.theme.BorderLight
import com.braincademy.junior.ui.theme.EasyGreen
import com.braincademy.junior.ui.theme.HardRed
import com.braincademy.junior.ui.theme.Purple60
import com.braincademy.junior.ui.theme.Purple80
import com.braincademy.junior.ui.theme.SurfaceLight
import com.braincademy.junior.ui.theme.TextPrimary
import com.braincademy.junior.ui.theme.TextSecondary
import com.braincademy.junior.viewmodel.GameViewModel

@Composable
fun GameScreen(
    viewModel: GameViewModel,
    onGameOver: () -> Unit,
) {
    LaunchedEffect(viewModel.currentQuestionIndex, viewModel.answered) {
        if (viewModel.isGameOver) {
            onGameOver()
        }
    }

    Box(
        modifier =
            Modifier
                .fillMaxSize()
                .background(Background),
    ) {
        Column(
            modifier =
                Modifier
                    .fillMaxSize()
                    .padding(16.dp),
        ) {
            // HUD
            Card(
                shape = RoundedCornerShape(12.dp),
                elevation = CardDefaults.cardElevation(2.dp),
            ) {
                Row(
                    modifier =
                        Modifier
                            .fillMaxWidth()
                            .padding(horizontal = 20.dp, vertical = 12.dp),
                    horizontalArrangement = Arrangement.SpaceBetween,
                    verticalAlignment = Alignment.CenterVertically,
                ) {
                    Column {
                        Text(
                            "${viewModel.timeLeft}",
                            fontSize = 28.sp,
                            fontWeight = FontWeight.Bold,
                            color = if (viewModel.timeLeft <= 5) HardRed else TextPrimary,
                        )
                        Text("seconds", fontSize = 10.sp, color = TextSecondary)
                    }
                    Text(
                        "${viewModel.currentQuestionIndex} / ${GameViewModel.TOTAL_QUESTIONS}",
                        fontSize = 14.sp,
                        color = TextSecondary,
                    )
                    Text(
                        "Score: ${viewModel.gameScore}",
                        fontSize = 16.sp,
                        fontWeight = FontWeight.SemiBold,
                        color = TextPrimary,
                    )
                }
            }

            Spacer(Modifier.height(8.dp))

            // Progress bar
            LinearProgressIndicator(
                progress = {
                    (viewModel.currentQuestionIndex - 1).toFloat() / GameViewModel.TOTAL_QUESTIONS
                },
                modifier =
                    Modifier
                        .fillMaxWidth()
                        .height(6.dp)
                        .clip(RoundedCornerShape(3.dp)),
                color = Purple60,
                trackColor = Color(0xFFE0E0E0),
            )

            Spacer(Modifier.height(16.dp))

            // Question card
            Card(
                modifier = Modifier.fillMaxWidth(),
                shape = RoundedCornerShape(16.dp),
                elevation = CardDefaults.cardElevation(4.dp),
            ) {
                when (val q = viewModel.currentQuestion) {
                    is GameQuestion.Standard -> StandardQuestionContent(q, viewModel)
                    is GameQuestion.Memory -> MemoryQuestionContent(q, viewModel)
                    null -> {}
                }
            }
        }

        // Feedback overlay
        AnimatedVisibility(
            visible = viewModel.showFeedback,
            enter = fadeIn(),
            exit = fadeOut(),
            modifier = Modifier.align(Alignment.Center),
        ) {
            Text(
                viewModel.feedbackText,
                fontSize = 40.sp,
                fontWeight = FontWeight.Bold,
                color = if (viewModel.feedbackIsCorrect) EasyGreen else HardRed,
            )
        }
    }
}

@Composable
private fun StandardQuestionContent(
    q: GameQuestion.Standard,
    viewModel: GameViewModel,
) {
    val question = q.question

    Column(
        modifier =
            Modifier
                .fillMaxWidth()
                .padding(24.dp),
        horizontalAlignment = Alignment.CenterHorizontally,
    ) {
        Text(
            question.label.uppercase(),
            fontSize = 11.sp,
            color = TextSecondary,
            letterSpacing = 2.sp,
        )

        Spacer(Modifier.height(16.dp))

        // Sequence display or question text
        if (question.sequence != null) {
            Row(
                horizontalArrangement = Arrangement.Center,
                verticalAlignment = Alignment.CenterVertically,
                modifier = Modifier.fillMaxWidth(),
            ) {
                question.sequence.forEachIndexed { index, item ->
                    if (index > 0) {
                        Text(
                            "\u2192",
                            fontSize = 16.sp,
                            color = TextSecondary,
                            modifier = Modifier.padding(horizontal = 4.dp),
                        )
                    }
                    Box(
                        modifier =
                            Modifier
                                .size(50.dp)
                                .clip(RoundedCornerShape(10.dp))
                                .background(if (item == "?") Purple60.copy(alpha = 0.1f) else SurfaceLight)
                                .border(
                                    2.dp,
                                    if (item == "?") Purple60 else BorderLight,
                                    RoundedCornerShape(10.dp),
                                ),
                        contentAlignment = Alignment.Center,
                    ) {
                        Text(
                            item,
                            fontSize = 18.sp,
                            fontWeight = FontWeight.Bold,
                            color = if (item == "?") Purple60 else TextPrimary,
                        )
                    }
                }
            }
        } else {
            Text(
                question.questionText ?: "",
                fontSize = 22.sp,
                fontWeight = FontWeight.SemiBold,
                textAlign = TextAlign.Center,
                color = TextPrimary,
            )
        }

        Spacer(Modifier.height(24.dp))

        // Answer buttons in 2-column grid
        val answers = question.answers
        for (row in answers.chunked(2)) {
            Row(
                modifier = Modifier.fillMaxWidth(),
                horizontalArrangement = Arrangement.spacedBy(10.dp),
            ) {
                row.forEachIndexed { _, answer ->
                    val globalIndex = answers.indexOf(answer)
                    val isCorrect = answer == question.correctAnswer
                    val isSelected = viewModel.selectedAnswerIndex == globalIndex

                    val bgColor =
                        when {
                            viewModel.showCorrect && isCorrect -> EasyGreen.copy(alpha = 0.15f)
                            isSelected && !isCorrect -> HardRed.copy(alpha = 0.15f)
                            else -> Color.White
                        }
                    val borderColor =
                        when {
                            viewModel.showCorrect && isCorrect -> EasyGreen
                            isSelected && !isCorrect -> HardRed
                            else -> BorderLight
                        }
                    val textColor =
                        when {
                            viewModel.showCorrect && isCorrect -> EasyGreen
                            isSelected && !isCorrect -> HardRed
                            else -> TextPrimary
                        }

                    Box(
                        modifier =
                            Modifier
                                .weight(1f)
                                .clip(RoundedCornerShape(12.dp))
                                .background(bgColor)
                                .border(2.dp, borderColor, RoundedCornerShape(12.dp))
                                .clickable(enabled = !viewModel.answered) {
                                    viewModel.selectAnswer(globalIndex)
                                }
                                .padding(16.dp),
                        contentAlignment = Alignment.Center,
                    ) {
                        Text(
                            answer,
                            fontSize = 16.sp,
                            fontWeight = FontWeight.Medium,
                            color = textColor,
                            textAlign = TextAlign.Center,
                        )
                    }
                }
                // Pad if odd number of answers
                if (row.size == 1) {
                    Spacer(Modifier.weight(1f))
                }
            }
            Spacer(Modifier.height(10.dp))
        }
    }
}

@Composable
private fun MemoryQuestionContent(
    q: GameQuestion.Memory,
    viewModel: GameViewModel,
) {
    val mem = q.memoryQuestion

    Column(
        modifier =
            Modifier
                .fillMaxWidth()
                .padding(24.dp),
        horizontalAlignment = Alignment.CenterHorizontally,
    ) {
        Text(
            "MEMORY GRID",
            fontSize = 11.sp,
            color = TextSecondary,
            letterSpacing = 2.sp,
        )

        Spacer(Modifier.height(12.dp))

        Text(
            if (viewModel.memoryShowPhase) {
                "Remember the highlighted cells!"
            } else {
                "Tap the ${mem.highlighted.size} cells that were highlighted!"
            },
            fontSize = 18.sp,
            fontWeight = FontWeight.SemiBold,
            textAlign = TextAlign.Center,
            color = TextPrimary,
        )

        Spacer(Modifier.height(20.dp))

        // Grid
        val totalCells = mem.gridSize * mem.gridSize
        LazyVerticalGrid(
            columns = GridCells.Fixed(mem.gridSize),
            horizontalArrangement = Arrangement.spacedBy(8.dp),
            verticalArrangement = Arrangement.spacedBy(8.dp),
            modifier = Modifier.width((mem.gridSize * 68).dp),
        ) {
            items(totalCells) { index ->
                val isHighlighted = index in mem.highlighted
                val isSelected = index in viewModel.memorySelectedCells
                val isCorrectPick = isSelected && isHighlighted
                val isWrongPick = isSelected && !isHighlighted
                val showAsHighlighted = viewModel.memoryShowPhase && isHighlighted
                val revealMissed = viewModel.memoryDone && isHighlighted && !isSelected

                val bgColor =
                    when {
                        showAsHighlighted -> Purple60
                        isCorrectPick -> EasyGreen
                        isWrongPick -> HardRed
                        revealMissed -> Purple60.copy(alpha = 0.5f)
                        else -> SurfaceLight
                    }
                val borderColor =
                    when {
                        showAsHighlighted -> Purple80
                        isCorrectPick -> EasyGreen
                        isWrongPick -> HardRed
                        else -> BorderLight
                    }

                Box(
                    modifier =
                        Modifier
                            .aspectRatio(1f)
                            .clip(RoundedCornerShape(10.dp))
                            .background(bgColor)
                            .border(2.dp, borderColor, RoundedCornerShape(10.dp))
                            .clickable(enabled = !viewModel.memoryShowPhase && !viewModel.answered) {
                                viewModel.selectMemoryCell(index)
                            },
                )
            }
        }
    }
}
