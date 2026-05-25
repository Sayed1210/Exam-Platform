const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL;

export async function beforeStartExam(token: string) {
  try {
    const res = await fetch(
      `${API_BASE_URL}/exams/before-start?token=${encodeURIComponent(token)}`
    );

    const text = await res.text();

    let result = null;

    try {
      result = text ? JSON.parse(text) : null;
    } catch {
      result = null;
    }

    if (!res.ok) {
      return {
        success: false,
        message: result?.message || "Failed to load exam",
        data: null,
      };
    }

    return {
      success: true,
      data: result,
    };
  } 
  catch {
    return {
      success: false,
      message: "Something went wrong",
      data: null,
    };
  }
}

export async function startExam(
  examId: number,
  candidateId: number
) {
  try {
    const res = await fetch(
      `${API_BASE_URL}/exams/${examId}/start`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          candidateId,
        }),
      }
    );

    const text = await res.text();

    let result = null;

    try {
      result = text ? JSON.parse(text) : null;
    } catch {
      result = null;
    }

    if (!res.ok) {
      return {
        success: false,
        message: result?.message || "Failed to start exam",
        data: null,
      };
    }

    return {
      success: true,
      data: result,
    };
  } catch {
    return {
      success: false,
      message: "Unable to connect to server",
      data: null,
    };
  }
}

export async function submitExam(
  examId: number,
  candidateId: number,
  answers: {questionId: number; choiceId: number;}[]
) {
  try {
    const res = await fetch(
      `${API_BASE_URL}/exams/${examId}/submit`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          candidateId,
          answers,
        }),
      }
    );

    const text = await res.text();

    let result = null;

    try {
      result = text ? JSON.parse(text) : null;
    } catch {
      result = null;
    }

    if (!res.ok) {
      return {
        success: false,
        message: result?.message || "Failed to submit exam",
      };
    }

    return {
      success: true,
      message: "Exam submitted successfully",
    };
  } catch {
    return {
      success: false,
      message: "Something went wrong",
    };
  }
}

export async function saveAnswer(
  examId: number,
  candidateId: number,
  questionId: number,
  choiceId: number
) {
  try {
    const res = await fetch(
      `${API_BASE_URL}/exams/${examId}/answers`,
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          candidateId,
          questionId,
          choiceId,
        }),
      }
    );

    const text = await res.text();

    let result = null;

    try {
      result = text ? JSON.parse(text) : null;
    } catch {
      result = null;
    }

    if (!res.ok) {
      return {
        success: false,
        message: result?.message || "Failed to save answer",
      };
    }

    return {
      success: true,
      message: "Answer saved successfully",
    };
  } catch {
    return {
      success: false,
      message: "Something went wrong",
    };
  }
}

export async function getSavedAnswers(
  examId: number,
  candidateId: number
) {
  try {
    const res = await fetch(
      `${API_BASE_URL}/exams/${examId}/answers/${candidateId}`
    );

    const text = await res.text();

    let result = null;

    try {
      result = text ? JSON.parse(text) : null;
    } catch {
      result = null;
    }

    if (!res.ok) {
      return {
        success: false,
        message: result?.message || "Failed to load saved answers",
        answers: [],
      };
    }

    return {
      success: true,
      answers: result?.answers || [],
    };
  } catch {
    return {
      success: false,
      message: "Something went wrong",
      answers: [],
    };
  }
}
