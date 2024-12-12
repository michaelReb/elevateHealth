## #########################################################################  ##
# function 1: recalculate_main_diagonal 
# 
# step 1: All main diagonal elements are set to 0
# step 2: For each row the main diagonal is set so that row sum adds up to 1
# -- validated --
## ########################################################################## ##

recalculate_main_diagonal <- function(rawMatrix)
{
for (k in 1:nrow(rawMatrix)) {
  rawMatrix[k, k] <- 0
  rawMatrix[k, k] <- 1 - sum(rawMatrix[k, ])
}
return(rawMatrix)
}

############################################################################# ##
# function 2: check_matrix_row_sum
# 
# step 1: Check whether each row sum is 1.
# step 2: Return (Failure if not)
# -- validated --
############################################################################# ##


check_matrix_row_sum <- function(rawMatrix)
{
  for (k in 1:nrow(rawMatrix)) {
    if (sum(rawMatrix[k, ]) != 1) {
    cat("Probabilities in row", k, "!= 1 - Value:", sum(rawMatrix[k, ]), "\n")
    return(FALSE)
  } else {return(TRUE)}
  }
}

############################################################################# ##
# function 3: model_diseasestates
#
# step 1: Define Transition States
# step 2: calculate timeevol
# step 3: return timeevol
#
############################################################################ ##

model_diseasestates <- function(variables) {

# step 1:
# u: uncontrolled; c: controlled; s: symptom-free
  
  uu <- variables[1]
  uc <- variables[2]
  us <- variables[3]
  cu <- variables[4]
  cc <- variables[5]
  cs <- variables[6]
  su <- variables[7]
  sc <- variables[8]
  ss <- variables[9]
  
  transMatrix <- base::matrix(c(uu, uc, us, cu, cc, cs, su, sc, ss), nrow = 3, ncol = 3, byrow = TRUE)
  
  # print(transMatrix)
  # Normalization of Matrix transMatrix
  
  # transMatrix_norm <- base::prop.table(transMatrix, margin = 1)
  # transMatrix_norm2 <- normalize_rows_l1(transMatrix)
  # 
  # print(transMatrix_norm)
  # print(transMatrix_norm2)
 
  initialState <- c(0.614, 0.3088, 0.0772)  #todo initial state not hard coded in function 
  
  timeevol <- initialState
  
  for (k in 1:number_of_cycles_t) {
    #  Original Version
    timeevol <- c(timeevol, as.vector(initialState %*% matrixdist::matrix_power(k, transMatrix)))
    
    # transposed version of initialState
    # timeevol <- c(timeevol, as.vector(t(initialState) %*% matrixdist::matrix_power(k, transMatrix_norm)))
    # expm Version
    # timeevol <- c(timeevol, as.vector(initialState %*% (transMatrix_norm2 %^% k)))   
      }
  
  #timeevol_56 <- timeevol
  # print(timeevol_56)
  timeevol <- base::matrix(timeevol, nrow = k + 1, ncol = length(initialState), byrow = TRUE)
  return(timeevol)
}

############################################################################## ##
# function 4: Residual function
#
# step 1: Calculate sum of squared deviations of estimated to hypothetical values  
# step 2: Return sum of squared deviations
#
############################################################################## ##

# Residual function

residual <- function(variables) {
  data <- df_diseasestates_input[1:number_of_cycles_t, ]
  return(sum((data - model_diseasestates(variables))^2))
}

############################################################################## ##
#  
# function 5: timeseries_prob
# 
# step 1: dynamic imputation in column 9 for rows 1 to 8
# step 2: call "function 1: recalculate_main_diagonal"
#
# -- validated --
############################################################################## ##

timeseries_prob <- function(transMatrix, x0, timelength) {
  n_states <- length(x0)
  result <- matrix(0, nrow = timelength, ncol = n_states)
  x_k <- x0 #todo sprechender Name für x_k
  result[1, ] <- x_k
  
  for (k in 2:timelength) {
    if (k < 1000) {
      transMatrix[1:8, 9] <- remission_prob[k, ]            # step 1: dynamic imputation in column 9 for rows 1 to 8
      transMatrix <- recalculate_main_diagonal(transMatrix) # step 2
    }
    x_k <- x_k %*% transMatrix
    result[k, ] <- x_k
  }
  return(result)
}

# Code zum Testen der Funktion
# x0 <- c(1, 0, 0, 0, 0, 0, 0, 0, 0)
# timelength <- 8000
# t <- timeseries_prob(transMatrix, x0, timelength)
# t (8000 Zeilen 9 Spalten)
# function timeseries_MC_vec has been removed here.

############################################################################## ##
#                                          
#  function 6: steadystate_MC              
#  Steady state from Monte Carlo           
#  function: steadystate_MC               
#
#  -- not yet validated --                                         
############################################################################# ##

 steadystate_MC <- function(transMatrix, x0, timelength, n_patients, simulationcycles) {
  result <- matrix(0, nrow = ((simulationcycles+1) * timelength), ncol = length(x0)) # leere Matrix mit 24.000 Zeilen erwartet waren 16.000
  t <- timeseries_prob(transMatrix, x0, timelength)
  
  for (k in 1:((simulationcycles * timelength))) {
    result[k:(k + timelength - 1), ] <- result[k:(k + timelength - 1), ] + t
    result[(k + timelength):nrow(result), ] <- result[(k + timelength):nrow(result), ] + t[nrow(t), ] #dasselbe aber ergänze den wert aus der letzten Zeile von t
  }
  
  return(result[1:k, ])
}

# Alternative

#steadystate_MC <- function(transMatrix, x0, timelength, n_patients, simulationcycles) {
#  result <- matrix(0, nrow = (simulationcycles + 1) * timelength, ncol = length(x0))
#  t <- timeseries_prob(transMatrix, x0, timelength)
#  for (k in 1:(simulationcycles * timelength)) {
#    result[k:(k + timelength), ] <- result[k:(k + timelength), ] + t
#    result[(k + timelength + 1):nrow(result), ] <- result[(k + timelength + 1):nrow(result), ] + t[nrow(t), ]
#  }
#  return(list(result[1:k, ], t))
#}


# function test code
#
# x0 <- c(1, 0, 0, 0, 0, 0, 0, 0, 0)
# timelength <- 8000
# n_patients <- 10
# simulationcycles <- 2
# steadystate_MC (transMatrix, x0, timelength, n_patients, simulationcycles)
# xxx1 <- matrix(0, nrow = ((simulationcycles) * timelength), ncol = length(x0))
# xxx1


############################################################################# ##
#
#  function 7: get_steadystate
#  Calculate steady state
#  Function: get_steadystate
#
############################################################################# ##

#todo parametrisieren --> Parameter eventuell aus der Funktion ziehen und Werte global steuern. 

get_steadystate <- function(transMatrix) {
  x0 <- c(1, 0, 0, 0, 0, 0, 0, 0, 0)
  timelength <- 8000
  n_patients <- 10
  simulationcycles <- 1
  result <- steadystate_MC(transMatrix, x0, timelength, n_patients, simulationcycles) # call function steadystate_MC
  normvec <- apply(result, 1, sum)
  result_norm <- result / matrix(normvec, nrow = length(normvec), ncol = ncol(result), byrow = TRUE)
  result_norm_noremission <- result_norm[nrow(result_norm), 1:8] / sum(result_norm[nrow(result_norm), 1:8]) * 100 
  return(result_norm_noremission) 
}

############################################################################# ##
#
#  function 8: reshape_steadystate_variables
#  step 1: calculate mean_UCT
#  step 2: calculate share_highUCT
#  step 3: define steadystate_df & colnames
#  step 4: reduce number of states by group aggregation
#
############################################################################# ##

# Derive output variables ####

reshape_steadystate_variables <- function(steadystate, states, UCTMatrix, cutoff_highUCT) {
  mean_UCT <- sum(steadystate / 100 * UCTMatrix[1, 1:8])
  share_highUCT <- sum((UCTMatrix[1, 1:8] >= cutoff_highUCT) * steadystate)
  steadystate_df <- as.data.frame(matrix(steadystate, nrow = 1, ncol = 8, byrow = TRUE))
  colnames(steadystate_df) <- states[1:8]
  aggregated_states_df <- data.frame(
    Symptomatic = steadystate_df$Symptomatic + steadystate_df$No_diagnosis_virt,
    Diagnosed = steadystate_df$Diagnosed + steadystate_df$No_treatment_virt + steadystate_df$Treatment_start,
    Treated_uncontrolled = steadystate_df$Uncontrolled,
    Treated_controlled = steadystate_df$Controlled,
    Treated_symptom_free = steadystate_df$Symptom_free
  )
  return(list(mean_UCT = mean_UCT, share_highUCT = share_highUCT, aggregated_states_df = aggregated_states_df))
}

############################################################################# ##

