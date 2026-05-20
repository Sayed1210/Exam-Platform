"use client";

import { useMemo, useState } from "react";
import { FormValidation } from "@/schemas/form-validation";
import { createQuestionRequestSchema } from "@/schemas/requests/create-question-request";
import { updateQuestionRequestSchema } from "@/schemas/requests/update-question-request";
import type { QuestionChoice, Question, APIQuestion } from "@/types/question";
import type { APITopic } from "@/types/question";
import {uploadImage} from "@/services/questionService";
import { getImageUrl } from "@/lib/api";
import { ArrowUpTrayIcon } from "@heroicons/react/16/solid";

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
      {/* Topic */}
      <label className="block">
        <div className="block">
          <span className="text-label">Topic</span>
          <select
            value={topicId}
            onChange={(e) => setTopicId(Number(e.target.value))}
            className="select-input"
          >
            {topics.map((t) => (
              <option key={t.id} value={t.id}>
                {t.title}
              </option>
            ))}
          </select>
        </div>
      </label>
      {/* Question */}
      <label className="block">
        <span className="text-label">
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
          placeholder="Enter question..."
          className="
            mt-2 min-h-24 w-full resize-y
            input
          "
        />

        {errors.text ? (
          <p className="mt-1 text-error">
            {errors.text}
          </p>
        ) : null}
      </label>
      {/* Question Image */}
      <label className="block">
        <span className="text-label">
          Question Image
          <span className="ml-1 text-gray">
            (optional)
          </span>
        </span>

        {/* upload img */}
        <div className="mt-3">
          <label
            className="
              flex h-32 w-full cursor-pointer flex-col
              items-center justify-center gap-2 rounded-2xl
              border-2 border-dashed border-slate-300
              bg-slate-50 transition
              hover:border-red-400 hover:bg-red-50
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
              <ArrowUpTrayIcon className="icon-big text-slate-500" />
            </div>

            <div className="text-center">
              <p className="text-muted">
                {isUploading
                  ? "Uploading..."
                  : "Click to upload an image"}
              </p>

              <p className="mt-1 text-muted text-xs text-slate-400">
                PNG, JPG, WEBP
              </p>
            </div>
          </label>
        </div>
        {/* preview img */}
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

      {/* Choices */}
      <fieldset>
        <legend className="text-label">
          Options{" "}
          <span className="text-gray">
            (select the correct answer)
          </span>
        </legend>

        <div className="mt-3 space-y-3">
          {errors.choicesMessage ? (
            <p className="text-error">
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
                  className="mt-3 h-4 w-4 accent-emerald-600 cursor-pointer"
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
                    className="input"
                  />

                  <label
    className={`
      flex h-11 cursor-pointer items-center
      justify-center rounded-lg border
      px-4 text-sm font-medium transition
      ${hasText
        ? "cursor-not-allowed bg-slate-100 text-slate-400"
        : "border-slate-200 bg-white hover:border-red-400 hover:bg-red-50"}
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
            btn-secondary
          "
        >
          Cancel
        </button>

        <button
          type="submit"
          className="
            btn-primary
          "
          disabled={isUploading}
        >
          Save Question
        </button>
      </div>
    </form>
  );
}
