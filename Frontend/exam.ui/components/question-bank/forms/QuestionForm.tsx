"use client";

import { useMemo, useState } from "react";
import { FormValidation } from "@/schemas/form-validation";
import { createQuestionRequestSchema } from "@/schemas/requests/create-question-request";
import { updateQuestionRequestSchema } from "@/schemas/requests/update-question-request";
import type { QuestionChoice, Question, APIQuestion } from "@/types/question";
import type { APITopic } from "@/types/question";
import {uploadImage} from "@/services/questionService";
import { getImageUrl } from "@/lib/api";

type QuestionFormProps = {
  topics: APITopic[];  
  question?: APIQuestion | null;
  onCancel: () => void;
 onSubmit: (data: any) => void; 
};

type ChoiceDraft = {
  text: string;
  imageUrl: string;
};

type FormErrors = {
  text?: string;
  choicesMessage?: string;
};

const optionLabels = ["A", "B", "C", "D"];

function createBlankChoices(): ChoiceDraft[] {
  return optionLabels.map(() => ({
    text: "",
    imageUrl: "",
  }));
}

export default function QuestionForm({
  topics,
  question,
  onCancel,
  onSubmit,
}: QuestionFormProps) {
  const initialChoices = useMemo(() => {
    if (!question) {
      return createBlankChoices();
    }

    return question.choices.map((choice) => ({
      text: choice.text ?? "",
      imageUrl: choice.imageUrl ?? "",
    }));
  }, [question]);

const [topicId, setTopicId] = useState<number>(
  question ? Number(question.topicId) : topics[0]?.id ?? 0
);

  const [text, setText] = useState(
    question?.text ?? ""
  );

  const [imageUrl, setImageUrl] = useState(
    question?.imageUrl ?? ""
  );

  const [choices, setChoices] =
    useState<ChoiceDraft[]>(initialChoices);

  const [correctChoiceIndex, setCorrectChoiceIndex] = useState(
    Math.max(
      question?.choices.findIndex((choice) => choice.isCorrect) ?? 0,
      0
    )
  );

  const [isUploading, setIsUploading] = useState(false);

  const [errors, setErrors] = useState<FormErrors>({});

  function updateChoice(
    choiceIndex: number,
    field: keyof ChoiceDraft,
    value: string
  ) {
    setChoices((currentChoices) =>
      currentChoices.map((choice, index) =>
        index === choiceIndex
          ? { ...choice, [field]: value }
          : choice
      )
    );

    setErrors((currentErrors) => {
      return {
        ...currentErrors,
        choicesMessage: undefined,
      };
    });
  }

  function handleSubmit(
    event: React.SyntheticEvent<HTMLFormElement>
  ) {
    event.preventDefault();
     console.log("handleSubmit called, topicId:", topicId, "text:", text);
  console.log("choices:", JSON.stringify(choices));

    const questionPayload = {
      topicId: topicId,
      text,
      imageUrl,
      choices: choices.map((choice, index) => {
        const trimmedText = choice.text.trim();
        const trimmedImageUrl = choice.imageUrl.trim();
        const choiceId = question?.choices[index]?.id;

        if (trimmedText && !trimmedImageUrl) {
          return {
            ...(choiceId ? { id: choiceId } : {}),
            text: choice.text,
            isCorrect: index === correctChoiceIndex,
          };
        }

        if (!trimmedText && trimmedImageUrl) {
          return {
            ...(choiceId ? { id: choiceId } : {}),
            imageUrl: choice.imageUrl,
            isCorrect: index === correctChoiceIndex,
          };
        }

        return {
          ...(choiceId ? { id: choiceId } : {}),
          text: choice.text,
          imageUrl: choice.imageUrl,
          isCorrect: index === correctChoiceIndex,
        };
      }),
    };

    const result = question
      ? FormValidation(updateQuestionRequestSchema, {
          id: String(question.id),
          ...questionPayload,
        })
      : FormValidation(createQuestionRequestSchema, questionPayload);
      console.log("Validation result:", result);

    if (!result.success) {
        console.log("Validation errors:", result.errors);
      setErrors({
        text: result.errors.text,
        choicesMessage: result.errors.choices,
      });
      return;
    }
onSubmit({
  topicId: result.data.topicId,
  text: result.data.text,
  imageUrl: result.data.imageUrl ?? null,
  choices: result.data.choices.map((choice) => ({
    text: choice.text ?? null,
    imageUrl: choice.imageUrl ?? null,
    isCorrect: choice.isCorrect,
  })),
} as any);
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-5">
      <label className="block">
       

<div className="block">
  <span className="text-sm font-semibold text-slate-900">Topic</span>
  <select
    value={topicId}
    onChange={(e) => setTopicId(Number(e.target.value))}
    className="mt-2 h-11 w-full rounded-lg border border-slate-200 bg-white px-4 text-sm text-slate-900 outline-none transition focus:border-blue-500 focus:ring-2 focus:ring-blue-100"
  >
    {topics.map((t) => (
      <option key={t.id} value={t.id}>
        {t.title}
      </option>
    ))}
  </select>
</div>
      </label>

      <label className="block">
        <span className="text-sm font-semibold text-slate-900">
          Question Statement
        </span>

        <textarea
          value={text}
          onChange={(event) => {
            setText(event.target.value);
            setErrors((currentErrors) => ({
              ...currentErrors,
              text: undefined,
            }));
          }}
          placeholder="Enter the question..."
          className="
            mt-2 min-h-24 w-full resize-y rounded-lg
            border border-slate-200 px-4 py-3 text-sm
            text-slate-900 outline-none transition
            placeholder:text-slate-400
            focus:border-blue-500 focus:ring-2
            focus:ring-blue-100
          "
        />

        {errors.text ? (
          <p className="mt-2 text-sm text-red-600">
            {errors.text}
          </p>
        ) : null}
      </label>

     <label className="block">
  <span className="text-sm font-semibold text-slate-900">
    Question Image
    <span className="ml-1 font-medium text-slate-400">
      (optional)
    </span>
  </span>

  <div className="mt-3">
    <label
      className="
        flex h-32 w-full cursor-pointer flex-col
        items-center justify-center gap-2 rounded-2xl
        border-2 border-dashed border-slate-300
        bg-slate-50 transition
        hover:border-blue-400 hover:bg-blue-50
      "
    >
      <input
        type="file"
        accept="image/*"
        className="hidden"
        onChange={async (event) => {
          const file = event.target.files?.[0];

          if (!file) return;

          setIsUploading(true);

          try {
            const response = await uploadImage(file);
            setImageUrl(response.imageUrl);
          } catch (error) {
            console.error(error);
          } finally {
            setIsUploading(false);
          }
        }}
      />

      <div
        className="
          flex h-12 w-12 items-center justify-center
          rounded-full bg-white shadow-sm
        "
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          className="h-6 w-6 text-slate-500"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
          strokeWidth={2}
        >
          <path
            strokeLinecap="round"
            strokeLinejoin="round"
            d="M3 15a4 4 0 014-4h1m4-4l4 4m0 0l4-4m-4 4V3"
          />
        </svg>
      </div>

      <div className="text-center">
        <p className="text-sm font-medium text-slate-700">
          {isUploading
            ? "Uploading..."
            : "Click to upload an image"}
        </p>

        <p className="mt-1 text-xs text-slate-400">
          PNG, JPG, WEBP
        </p>
      </div>
    </label>
  </div>

  {imageUrl ? (
    <img
      src={getImageUrl(imageUrl)}
      alt="Question preview"
      className="
        mt-4 max-h-56 rounded-xl border
        border-slate-200 shadow-sm
      "
    />
  ) : null}
</label>

      <fieldset>
        <legend className="text-sm font-semibold text-slate-900">
          Options{" "}
          <span className="font-medium text-slate-400">
            (select the correct answer)
          </span>
        </legend>

        <div className="mt-3 space-y-3">
          {errors.choicesMessage ? (
            <p className="text-sm text-red-600">
              {errors.choicesMessage}
            </p>
          ) : null}

          {choices.map((choice, index) => {
            const hasText = Boolean(choice.text.trim());
            const hasImage = Boolean(
              choice.imageUrl.trim()
            );

            return (
              <div
                key={optionLabels[index]}
                className="
                  grid grid-cols-[auto_1fr]
                  gap-x-3 gap-y-2
                "
              >
                <input
                  type="radio"
                  name="correctChoice"
                  checked={
                    correctChoiceIndex === index
                  }
                  onChange={() =>
                    setCorrectChoiceIndex(index)
                  }
                  className="mt-3 h-4 w-4 accent-blue-600"
                  aria-label={`Mark option ${optionLabels[index]} as correct`}
                />

                <div className="grid gap-2 sm:grid-cols-[1fr_180px]">
                  <input
                    value={choice.text}
                    disabled={hasImage}
                    onChange={(event) =>
                      updateChoice(
                        index,
                        "text",
                        event.target.value
                      )
                    }
                    placeholder={`Option ${optionLabels[index]}`}
                    className="
                      h-11 rounded-lg border border-slate-200
                      px-4 text-sm text-slate-900 outline-none
                      transition placeholder:text-slate-400
                      focus:border-blue-500 focus:ring-2
                      focus:ring-blue-100
                      disabled:bg-slate-100
                      disabled:text-slate-400
                      disabled:cursor-not-allowed
                    "
                  />

                  <label
  className={`
    flex h-11 cursor-pointer items-center
    justify-center rounded-lg border
    px-4 text-sm font-medium transition
    ${hasText
      ? "cursor-not-allowed bg-slate-100 text-slate-400"
      : "border-slate-200 bg-white hover:border-blue-400 hover:bg-blue-50"}
  `}
>
  <input
    type="file"
    accept="image/*"
    disabled={hasText}
    className="hidden"
    onChange={async (event) => {
      const file = event.target.files?.[0];

      if (!file) return;

      try {
        const response = await uploadImage(file);

        updateChoice(
          index,
          "imageUrl",
          response.imageUrl
        );
      } catch (error) {
        console.error(error);
      }
    }}
  />

  {choice.imageUrl
    ? "Image Uploaded"
    : "Upload Image"}
</label>
                </div>

              </div>
            );
          })}
        </div>
      </fieldset>

      <div className="flex justify-end gap-3 border-t border-slate-100 pt-5">
        <button
          type="button"
          onClick={onCancel}
          className="
            bg-white text-black font-semibold
            px-6 py-2.5 rounded-full
            hover:brightness-90 transition
          "
        >
          Cancel
        </button>

        <button
          type="submit"
          className="
            bg-primary text-white font-semibold
            px-6 py-2.5 rounded-full
            hover:brightness-90 transition
          "
          disabled={isUploading}
        >
          Save Question
        </button>
      </div>
    </form>
  );
}
