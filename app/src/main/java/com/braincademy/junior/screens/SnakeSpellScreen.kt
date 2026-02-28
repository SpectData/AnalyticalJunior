package com.braincademy.junior.screens

import android.graphics.Paint
import android.graphics.Typeface
import androidx.compose.animation.AnimatedVisibility
import androidx.compose.animation.fadeIn
import androidx.compose.animation.fadeOut
import androidx.compose.foundation.Canvas
import androidx.compose.foundation.background
import androidx.compose.foundation.border
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Box
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.rememberScrollState
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.foundation.verticalScroll
import androidx.compose.material3.Card
import androidx.compose.material3.CardDefaults
import androidx.compose.material3.LinearProgressIndicator
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.runtime.LaunchedEffect
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.geometry.CornerRadius
import androidx.compose.ui.geometry.Offset
import androidx.compose.ui.geometry.Size
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.graphics.drawscope.DrawScope
import androidx.compose.ui.graphics.nativeCanvas
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.TextUnit
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.braincademy.junior.model.BattlefieldState
import com.braincademy.junior.model.GameQuestion
import com.braincademy.junior.model.GameStatus
import com.braincademy.junior.model.Snake
import com.braincademy.junior.model.SnakeSpellConstants
import com.braincademy.junior.model.Spell
import com.braincademy.junior.ui.theme.Background
import com.braincademy.junior.ui.theme.BorderLight
import com.braincademy.junior.ui.theme.EasyGreen
import com.braincademy.junior.ui.theme.GrassDark
import com.braincademy.junior.ui.theme.GrassLight
import com.braincademy.junior.ui.theme.HardRed
import com.braincademy.junior.ui.theme.Purple60
import com.braincademy.junior.ui.theme.SpellGold
import com.braincademy.junior.ui.theme.TextPrimary
import com.braincademy.junior.ui.theme.TextSecondary
import com.braincademy.junior.viewmodel.SnakeSpellViewModel

@Composable
fun SnakeSpellScreen(
    viewModel: SnakeSpellViewModel,
    onGameOver: () -> Unit,
) {
    LaunchedEffect(viewModel.battlefield.status) {
        if (viewModel.battlefield.status == GameStatus.GAME_OVER) {
            kotlinx.coroutines.delay(1500L)
            onGameOver()
        }
    }

    Box(
        modifier =
            Modifier
                .fillMaxSize()
                .background(Background),
    ) {
        Column(modifier = Modifier.fillMaxSize()) {
            // HUD
            BattleHud(viewModel.battlefield)

            // Battlefield Canvas
            BattlefieldCanvas(
                battlefield = viewModel.battlefield,
                modifier =
                    Modifier
                        .fillMaxWidth()
                        .weight(0.5f),
            )

            // Question Panel
            QuestionPanel(
                question = viewModel.currentQuestion,
                timeLeft = viewModel.questionTimeLeft,
                maxTime = viewModel.config.questionTimerSeconds,
                answered = viewModel.answered,
                selectedIndex = viewModel.selectedAnswerIndex,
                showCorrect = viewModel.showCorrect,
                onAnswerSelected = { viewModel.submitAnswer(it) },
                modifier =
                    Modifier
                        .fillMaxWidth()
                        .weight(0.5f)
                        .padding(horizontal = 12.dp, vertical = 8.dp),
            )
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
                fontSize = 36.sp,
                fontWeight = FontWeight.Bold,
                color = if (viewModel.feedbackIsCorrect) EasyGreen else HardRed,
            )
        }
    }
}

@Composable
private fun BattleHud(battlefield: BattlefieldState) {
    Card(
        shape = RoundedCornerShape(0.dp),
        elevation = CardDefaults.cardElevation(4.dp),
    ) {
        Row(
            modifier =
                Modifier
                    .fillMaxWidth()
                    .padding(horizontal = 16.dp, vertical = 10.dp),
            horizontalArrangement = Arrangement.SpaceBetween,
            verticalAlignment = Alignment.CenterVertically,
        ) {
            // Lives
            Row(verticalAlignment = Alignment.CenterVertically) {
                Text("\u2764\uFE0F", fontSize = 16.sp)
                Spacer(Modifier.width(4.dp))
                Text(
                    "${battlefield.lives}",
                    fontSize = 18.sp,
                    fontWeight = FontWeight.Bold,
                    color = if (battlefield.lives <= 1) HardRed else TextPrimary,
                )
            }

            // Wave
            Text(
                "Wave ${battlefield.currentWave}",
                fontSize = 16.sp,
                fontWeight = FontWeight.SemiBold,
                color = TextPrimary,
            )

            // Score
            Text(
                "Score: ${battlefield.score}",
                fontSize = 16.sp,
                fontWeight = FontWeight.Bold,
                color = Purple60,
            )
        }
    }
}

@Composable
private fun BattlefieldCanvas(
    battlefield: BattlefieldState,
    modifier: Modifier = Modifier,
) {
    Canvas(modifier = modifier) {
        val laneHeight = size.height / SnakeSpellConstants.NUM_LANES
        val fieldWidth = size.width

        // Draw lanes
        for (lane in 0 until SnakeSpellConstants.NUM_LANES) {
            val y = lane * laneHeight
            drawRect(
                color = if (lane % 2 == 0) GrassLight else GrassDark,
                topLeft = Offset(0f, y),
                size = Size(fieldWidth, laneHeight),
            )
            // Lane path (dirt road)
            drawRect(
                color = Color(0xFF8B7355).copy(alpha = 0.3f),
                topLeft = Offset(0f, y + laneHeight * 0.3f),
                size = Size(fieldWidth, laneHeight * 0.4f),
            )
        }

        // Draw wizards
        for (lane in 0 until SnakeSpellConstants.NUM_LANES) {
            drawWizard(lane, laneHeight)
        }

        // Draw snakes
        for (snake in battlefield.snakes) {
            drawSnake(snake, laneHeight, fieldWidth)
        }

        // Draw spells
        for (spell in battlefield.spells) {
            drawSpell(spell, laneHeight, fieldWidth)
        }

        // Wave transition overlay
        if (battlefield.status == GameStatus.WAVE_TRANSITION) {
            drawRect(color = Color.Black.copy(alpha = 0.5f), size = size)
            drawContext.canvas.nativeCanvas.drawText(
                "Wave ${battlefield.currentWave} Complete!",
                size.width / 2f,
                size.height / 2f,
                Paint().apply {
                    color = android.graphics.Color.WHITE
                    textSize = 60f
                    textAlign = Paint.Align.CENTER
                    typeface = Typeface.DEFAULT_BOLD
                    isAntiAlias = true
                },
            )
        }

        // Game over overlay
        if (battlefield.status == GameStatus.GAME_OVER) {
            drawRect(color = Color(0xCC000000), size = size)
            drawContext.canvas.nativeCanvas.drawText(
                "GAME OVER",
                size.width / 2f,
                size.height / 2f,
                Paint().apply {
                    color = android.graphics.Color.RED
                    textSize = 80f
                    textAlign = Paint.Align.CENTER
                    typeface = Typeface.DEFAULT_BOLD
                    isAntiAlias = true
                },
            )
        }
    }
}

private fun DrawScope.drawWizard(
    lane: Int,
    laneHeight: Float,
) {
    val centerX = 35f
    val centerY = lane * laneHeight + laneHeight / 2f
    val radius = laneHeight * 0.22f

    // Body (robe)
    drawCircle(
        color = Color(0xFF3498DB),
        radius = radius,
        center = Offset(centerX, centerY),
    )
    // Hat (triangle approximation with circle)
    drawCircle(
        color = Color(0xFF2C3E50),
        radius = radius * 0.55f,
        center = Offset(centerX, centerY - radius * 0.9f),
    )
    // Star on hat
    drawCircle(
        color = SpellGold,
        radius = radius * 0.15f,
        center = Offset(centerX, centerY - radius * 0.85f),
    )
}

private fun DrawScope.drawSnake(
    snake: Snake,
    laneHeight: Float,
    fieldWidth: Float,
) {
    val scale = fieldWidth / SnakeSpellConstants.FIELD_WIDTH
    val snakeX = snake.xPosition * scale
    val snakeY = snake.lane * laneHeight + laneHeight / 2f
    val segmentRadius = laneHeight * 0.16f

    // Body segments (3 circles trailing behind)
    for (i in 2 downTo 0) {
        drawCircle(
            color = snake.type.bodyColor.copy(alpha = 1f - i * 0.15f),
            radius = segmentRadius * (1f - i * 0.1f),
            center = Offset(snakeX + i * segmentRadius * 1.4f, snakeY),
        )
    }

    // Head (slightly larger, darker)
    drawCircle(
        color = snake.type.headColor,
        radius = segmentRadius * 1.1f,
        center = Offset(snakeX - segmentRadius * 0.3f, snakeY),
    )

    // Eyes
    drawCircle(
        color = Color.White,
        radius = segmentRadius * 0.25f,
        center = Offset(snakeX - segmentRadius * 0.5f, snakeY - segmentRadius * 0.3f),
    )
    drawCircle(
        color = Color.Black,
        radius = segmentRadius * 0.12f,
        center = Offset(snakeX - segmentRadius * 0.55f, snakeY - segmentRadius * 0.3f),
    )

    // HP bar for multi-HP snakes
    if (snake.type.baseHp > 1) {
        val barWidth = segmentRadius * 2.5f
        val barHeight = 6f
        val barX = snakeX - barWidth / 2f
        val barY = snakeY - segmentRadius * 1.6f
        val hpFraction = snake.hp.toFloat() / snake.type.baseHp

        // Background
        drawRoundRect(
            color = Color.Black.copy(alpha = 0.4f),
            topLeft = Offset(barX, barY),
            size = Size(barWidth, barHeight),
            cornerRadius = CornerRadius(3f),
        )
        // Health
        val hpColor =
            when {
                hpFraction > 0.5f -> EasyGreen
                hpFraction > 0.25f -> Color(0xFFF39C12)
                else -> HardRed
            }
        drawRoundRect(
            color = hpColor,
            topLeft = Offset(barX, barY),
            size = Size(barWidth * hpFraction, barHeight),
            cornerRadius = CornerRadius(3f),
        )
    }
}

private fun DrawScope.drawSpell(
    spell: Spell,
    laneHeight: Float,
    fieldWidth: Float,
) {
    val scale = fieldWidth / SnakeSpellConstants.FIELD_WIDTH
    val spellX = spell.xPosition * scale
    val spellY = spell.lane * laneHeight + laneHeight / 2f
    val radius = laneHeight * 0.1f

    // Glow
    drawCircle(
        color = SpellGold.copy(alpha = 0.3f),
        radius = radius * 2f,
        center = Offset(spellX, spellY),
    )
    // Core
    drawCircle(
        color = SpellGold,
        radius = radius,
        center = Offset(spellX, spellY),
    )
    // Bright center
    drawCircle(
        color = Color.White,
        radius = radius * 0.4f,
        center = Offset(spellX, spellY),
    )
}

@Composable
private fun QuestionPanel(
    question: GameQuestion.Standard?,
    timeLeft: Int,
    maxTime: Int,
    answered: Boolean,
    selectedIndex: Int,
    showCorrect: Boolean,
    onAnswerSelected: (Int) -> Unit,
    modifier: Modifier = Modifier,
) {
    val q = question?.question ?: return

    // Adapt font sizes for longer OC questions
    val questionLen = (q.questionText ?: "").length
    val questionFontSize =
        when {
            questionLen > 120 -> 13.sp
            questionLen > 70 -> 15.sp
            else -> 18.sp
        }
    val maxAnswerLen = q.answers.maxOf { it.length }
    val answerFontSize =
        when {
            maxAnswerLen > 40 -> 11.sp
            maxAnswerLen > 20 -> 12.sp
            else -> 14.sp
        }

    Card(
        modifier = modifier,
        shape = RoundedCornerShape(16.dp),
        elevation = CardDefaults.cardElevation(4.dp),
    ) {
        Column(
            modifier =
                Modifier
                    .fillMaxSize()
                    .padding(12.dp)
                    .verticalScroll(rememberScrollState()),
            horizontalAlignment = Alignment.CenterHorizontally,
        ) {
            // Timer bar
            Row(
                modifier = Modifier.fillMaxWidth(),
                verticalAlignment = Alignment.CenterVertically,
                horizontalArrangement = Arrangement.SpaceBetween,
            ) {
                LinearProgressIndicator(
                    progress = { timeLeft.toFloat() / maxTime },
                    modifier =
                        Modifier
                            .weight(1f)
                            .height(8.dp)
                            .clip(RoundedCornerShape(4.dp)),
                    color = if (timeLeft <= 3) HardRed else Purple60,
                    trackColor = Color(0xFFE0E0E0),
                )
                Spacer(Modifier.width(8.dp))
                Text(
                    "${timeLeft}s",
                    fontSize = 14.sp,
                    fontWeight = FontWeight.Bold,
                    color = if (timeLeft <= 3) HardRed else TextPrimary,
                )
            }

            Spacer(Modifier.height(6.dp))

            // Label
            Text(
                q.label.uppercase(),
                fontSize = 10.sp,
                color = TextSecondary,
                letterSpacing = 2.sp,
            )

            Spacer(Modifier.height(4.dp))

            // Question text or sequence
            if (q.sequence != null) {
                Row(
                    horizontalArrangement = Arrangement.Center,
                    verticalAlignment = Alignment.CenterVertically,
                    modifier = Modifier.fillMaxWidth(),
                ) {
                    q.sequence.forEachIndexed { index, item ->
                        if (index > 0) {
                            Text(
                                "\u2192",
                                fontSize = 12.sp,
                                color = TextSecondary,
                                modifier = Modifier.padding(horizontal = 2.dp),
                            )
                        }
                        Box(
                            modifier =
                                Modifier
                                    .height(36.dp)
                                    .width(36.dp)
                                    .clip(RoundedCornerShape(6.dp))
                                    .background(
                                        if (item == "?") {
                                            Purple60.copy(alpha = 0.1f)
                                        } else {
                                            Color(0xFFF0F4F8)
                                        },
                                    )
                                    .border(
                                        1.dp,
                                        if (item == "?") Purple60 else BorderLight,
                                        RoundedCornerShape(6.dp),
                                    ),
                            contentAlignment = Alignment.Center,
                        ) {
                            Text(
                                item,
                                fontSize = 13.sp,
                                fontWeight = FontWeight.Bold,
                                color = if (item == "?") Purple60 else TextPrimary,
                            )
                        }
                    }
                }
            } else {
                Text(
                    q.questionText ?: "",
                    fontSize = questionFontSize,
                    fontWeight = FontWeight.SemiBold,
                    textAlign = TextAlign.Center,
                    color = TextPrimary,
                )
            }

            Spacer(Modifier.height(8.dp))

            AnswerGrid(
                answers = q.answers,
                correctAnswer = q.correctAnswer,
                answerFontSize = answerFontSize,
                maxAnswerLen = maxAnswerLen,
                answered = answered,
                selectedIndex = selectedIndex,
                showCorrect = showCorrect,
                onAnswerSelected = onAnswerSelected,
            )
        }
    }
}

@Composable
private fun AnswerGrid(
    answers: List<String>,
    correctAnswer: String,
    answerFontSize: TextUnit,
    maxAnswerLen: Int,
    answered: Boolean,
    selectedIndex: Int,
    showCorrect: Boolean,
    onAnswerSelected: (Int) -> Unit,
) {
    val columns = if (maxAnswerLen > 30) 1 else 2

    var runningIndex = 0
    for (row in answers.chunked(columns)) {
        Row(
            modifier = Modifier.fillMaxWidth(),
            horizontalArrangement = Arrangement.spacedBy(8.dp),
        ) {
            row.forEachIndexed { rowIdx, answer ->
                val globalIndex = runningIndex + rowIdx
                AnswerButton(
                    answer = answer,
                    isCorrect = answer == correctAnswer,
                    isSelected = selectedIndex == globalIndex,
                    showCorrect = showCorrect,
                    answered = answered,
                    fontSize = answerFontSize,
                    onClick = { onAnswerSelected(globalIndex) },
                    modifier = Modifier.weight(1f),
                )
            }
            if (row.size < columns) {
                Spacer(Modifier.weight(1f))
            }
        }
        Spacer(Modifier.height(6.dp))
        runningIndex += row.size
    }
}

@Composable
private fun AnswerButton(
    answer: String,
    isCorrect: Boolean,
    isSelected: Boolean,
    showCorrect: Boolean,
    answered: Boolean,
    fontSize: TextUnit,
    onClick: () -> Unit,
    modifier: Modifier = Modifier,
) {
    val bgColor =
        when {
            showCorrect && isCorrect -> EasyGreen.copy(alpha = 0.15f)
            isSelected && !isCorrect -> HardRed.copy(alpha = 0.15f)
            else -> Color.White
        }
    val borderColor =
        when {
            showCorrect && isCorrect -> EasyGreen
            isSelected && !isCorrect -> HardRed
            else -> BorderLight
        }
    val textColor =
        when {
            showCorrect && isCorrect -> EasyGreen
            isSelected && !isCorrect -> HardRed
            else -> TextPrimary
        }

    Box(
        modifier =
            modifier
                .clip(RoundedCornerShape(10.dp))
                .background(bgColor)
                .border(2.dp, borderColor, RoundedCornerShape(10.dp))
                .clickable(enabled = !answered) { onClick() }
                .padding(horizontal = 10.dp, vertical = 8.dp),
        contentAlignment = Alignment.Center,
    ) {
        Text(
            answer,
            fontSize = fontSize,
            fontWeight = FontWeight.Medium,
            color = textColor,
            textAlign = TextAlign.Center,
        )
    }
}
