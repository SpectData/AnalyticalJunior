package com.braincademy.junior.screens

import androidx.compose.foundation.background
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Column
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.Spacer
import androidx.compose.foundation.layout.fillMaxSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.height
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.width
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Button
import androidx.compose.material3.ButtonDefaults
import androidx.compose.material3.OutlinedButton
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.text.style.TextAlign
import androidx.compose.ui.unit.dp
import androidx.compose.ui.unit.sp
import com.braincademy.junior.model.Category
import com.braincademy.junior.model.Difficulty
import com.braincademy.junior.ui.theme.Background
import com.braincademy.junior.ui.theme.EasyGreen
import com.braincademy.junior.ui.theme.HardRed
import com.braincademy.junior.ui.theme.MediumYellow
import com.braincademy.junior.ui.theme.SuperHardPurple
import com.braincademy.junior.ui.theme.TextPrimary
import com.braincademy.junior.ui.theme.TextSecondary

@Composable
fun DifficultyScreen(
    category: Category? = null,
    title: String? = null,
    onDifficultySelected: (Difficulty) -> Unit,
    onBack: () -> Unit,
) {
    val displayTitle = title ?: "${category?.displayName ?: ""} \u2014 Select Difficulty"
    Column(
        modifier =
            Modifier
                .fillMaxSize()
                .background(Background)
                .padding(20.dp),
    ) {
        OutlinedButton(onClick = onBack) {
            Text("\u2190 Back")
        }

        Spacer(Modifier.height(24.dp))

        Text(
            displayTitle,
            fontSize = 22.sp,
            fontWeight = FontWeight.Bold,
            color = TextPrimary,
            textAlign = TextAlign.Center,
            modifier = Modifier.fillMaxWidth(),
        )

        Spacer(Modifier.height(32.dp))

        Column(
            modifier = Modifier.fillMaxWidth(),
            horizontalAlignment = Alignment.CenterHorizontally,
            verticalArrangement = Arrangement.spacedBy(12.dp),
        ) {
            Row(
                horizontalArrangement = Arrangement.Center,
            ) {
                DifficultyButton("Easy", "+10 pts", EasyGreen) { onDifficultySelected(Difficulty.EASY) }
                Spacer(Modifier.width(12.dp))
                DifficultyButton("Medium", "+25 pts", MediumYellow) { onDifficultySelected(Difficulty.MEDIUM) }
            }
            Row(
                horizontalArrangement = Arrangement.Center,
            ) {
                DifficultyButton("Hard", "+50 pts", HardRed) { onDifficultySelected(Difficulty.HARD) }
                Spacer(Modifier.width(12.dp))
                DifficultyButton("Super Hard", "+100 pts", SuperHardPurple) {
                    onDifficultySelected(Difficulty.SUPER_HARD)
                }
            }
        }

        Spacer(Modifier.height(16.dp))

        Text(
            "Higher difficulty = more points but harder questions and less time!",
            fontSize = 13.sp,
            color = TextSecondary,
            textAlign = TextAlign.Center,
            modifier = Modifier.fillMaxWidth(),
        )
    }
}

@Composable
private fun DifficultyButton(
    label: String,
    subtitle: String,
    color: Color,
    onClick: () -> Unit,
) {
    Button(
        onClick = onClick,
        colors = ButtonDefaults.buttonColors(containerColor = color),
        shape = RoundedCornerShape(12.dp),
        modifier = Modifier.height(80.dp).width(105.dp),
    ) {
        Column(horizontalAlignment = Alignment.CenterHorizontally) {
            Text(label, fontWeight = FontWeight.Bold, fontSize = 16.sp, color = Color.White)
            Text(subtitle, fontSize = 11.sp, color = Color.White.copy(alpha = 0.8f))
        }
    }
}
