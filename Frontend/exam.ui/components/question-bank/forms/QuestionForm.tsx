"use client";

import { useMemo, useState } from "react";
import { FormValidation } from "@/schemas/form-validation";
import { createQuestionRequestSchema } from "@/schemas/requests/create-question-request";
import { updateQuestionRequestSchema } from "@/schemas/requests/update-question-request";
import type { APIQuestion } from "@/types/question";
import type { APITopic } from "@/types/question";
import { uploadImage } from "@/services/questionService";
import { getImageUrl } from "@/lib/api";
import { PhotoIcon, PlusIcon, XMarkIcon } from "@heroicons/react/24/outline";

type QuestionFormSubmitData = {
  topicId: number;
  text: string;
  imageUrl: string | null;
  choices: Array<{
    text: string | null;
    imageUrl: string | null;
    isCorrect: boolean;
  }>;
};

type QuestionFormProps = {
  topics: APITopic[];
  question?: APIQuestion | null;
  onCancel: () => void;
  onSubmit: (data: QuestionFormSubmitData) => void;
};

type ChoiceDraft = {
  text: string;
  imageUrl: string;
};

type FormErrors = {
  text?: string;
  choicesMessage?: string;
};

const minimumChoices = 2;
const defaultChoiceCount = 4;

function createBlankChoices(): ChoiceDraft[] {
  return Array.from({ length: defaultChoiceCount }, () => ({
    text: "",
    imageUrl: "",
  }));
}

function createBlankChoice(): ChoiceDraft {
  return {
    text: "",
    imageUrl: "",
  };
}

function getOptionLabel(index: number) {
  return String.fromCharCode(65 + index);
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

    const questionChoices = question.choices.map((choice) => ({
      text: choice.text ?? "",
      imageUrl: choice.imageUrl ?? "",
    }));

    return questionChoices.length >= minimumChoices
      ? questionChoices
      : [
          ...questionChoices,
          ...Array.from(
            { length: minimumChoices - questionChoices.length },
            createBlankChoice
          ),
        ];
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

  function addChoice() {
    setChoices((currentChoices) => [
      ...currentChoices,
      createBlankChoice(),
    ]);

    setErrors((currentErrors) => ({
      ...currentErrors,
      choicesMessage: undefined,
    }));
  }

  function removeChoice(choiceIndex: number) {
    if (choices.length <= minimumChoices) {
      return;
    }

    const nextChoiceCount = choices.length - 1;

    setChoices((currentChoices) =>
      currentChoices.filter((_, index) => index !== choiceIndex)
    );

    setCorrectChoiceIndex((currentIndex) => {
      if (currentIndex === choiceIndex) {
        return 0;
      }

      if (currentIndex > choiceIndex) {
        return currentIndex - 1;
      }

      return Math.min(currentIndex, nextChoiceCount - 1);
    });

    setErrors((currentErrors) => ({
      ...currentErrors,
      choicesMessage: undefined,
    }));
  }

  function handleSubmit(
    event: React.SyntheticEvent<HTMLFormElement>
  ) {
    event.preventDefault();

    const questionPayload = {
      topicId,
      text,
      imageUrl,
      choices: choices.map((choice, index) => {
        const choiceId = question?.choices[index]?.id;

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

    if (!result.success) {
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
    });
  };

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
      <div className="block">
        <div className="flex items-center gap-2">
          <span className="text-label">
            Question Image
            <span className="ml-1 text-gray">
              (optional)
            </span>
          </span>

          <input
            type="file"
            id="question-image"
            accept="image/*"
            className="hidden"
            onChange={async (event) => {
              const input = event.currentTarget;
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
                input.value = "";
              }
            }}
          />

          <label
            htmlFor="question-image"
            className="
              cursor-pointer rounded-lg border border-slate-100
              bg-white p-1.5 text-slate-400 transition
              hover:text-slate-600
            "
            aria-label="Upload question image"
            title={isUploading ? "Uploading..." : "Upload question image"}
          >
            <PhotoIcon className="h-4 w-4" />
          </label>
        </div>

        {imageUrl ? (
          <div className="group relative mt-3 w-fit">
            <img
              src={getImageUrl(imageUrl)}
              alt="Question preview"
              className="
                max-h-56 rounded-xl border border-slate-200
                bg-white shadow-sm
              "
            />
            <button
              type="button"
              onClick={() => setImageUrl("")}
              className="
                absolute right-2 top-2 rounded-full bg-slate-800/70
                p-1.5 text-white opacity-0 transition-opacity
                group-hover:opacity-100
              "
              aria-label="Remove question image"
            >
              <XMarkIcon className="h-4 w-4" />
            </button>
          </div>
        ) : null}
      </div>

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
            const optionLabel = getOptionLabel(index);
            const canRemoveChoice =
              choices.length > minimumChoices;

            return (
              <div
                key={`${optionLabel}-${index}`}
                className="
                  group
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
                  aria-label={`Mark option ${optionLabel} as correct`}
                />

                <div className="grid gap-2 sm:grid-cols-[1fr_auto]">
                  <input
                    value={choice.text}
                    onChange={(event) =>
                      updateChoice(
                        index,
                        "text",
                        event.target.value
                      )
                    }
                    placeholder={`Option ${optionLabel}`}
                    className="input"
                  />

                  <div className="flex items-center gap-2">
                    <input
                      type="file"
                      id={`choice-image-${index}`}
                      accept="image/*"
                      className="hidden"
                      onChange={async (event) => {
                        const input = event.currentTarget;
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
                        } finally {
                          input.value = "";
                        }
                      }}
                    />

                    <label
                      htmlFor={`choice-image-${index}`}
                      className="
                        cursor-pointer rounded-lg border border-slate-100 bg-white
                        p-1.5 text-slate-400 transition
                        hover:text-slate-600
                      "
                      aria-label={`Upload image for option ${optionLabel}`}
                      title={`Upload image for option ${optionLabel}`}
                    >
                      <PhotoIcon className="h-4 w-4" />
                    </label>

                    <button
                      type="button"
                      onClick={() => removeChoice(index)}
                      disabled={!canRemoveChoice}
                      className={`
                        rounded-lg p-1 text-slate-300 transition-colors
                        ${canRemoveChoice
                          ? "hover:text-red-500"
                          : "cursor-not-allowed opacity-40"}
                      `}
                      aria-label={`Remove option ${optionLabel}`}
                      title={
                        canRemoveChoice
                          ? `Remove option ${optionLabel}`
                          : "A question must have at least 2 choices"
                      }
                    >
                      <XMarkIcon className="h-4 w-4" />
                    </button>
                  </div>
                </div>

                {choice.imageUrl ? (
                  <div className="group relative col-start-2 w-fit">
                    <img
                      src={getImageUrl(choice.imageUrl)}
                      alt={`Option ${optionLabel} preview`}
                      className="
                        max-h-32 rounded-xl border border-slate-200
                        bg-white shadow-sm
                      "
                    />
                    <button
                      type="button"
                      onClick={() =>
                        updateChoice(index, "imageUrl", "")
                      }
                      className="
                        absolute right-1.5 top-1.5 rounded-full
                        bg-slate-800/70 p-1 text-white opacity-0
                        transition-opacity group-hover:opacity-100
                      "
                      aria-label={`Remove image for option ${optionLabel}`}
                    >
                      <XMarkIcon className="h-3.5 w-3.5" />
                    </button>
                  </div>
                ) : null}

              </div>
            );
          })}

          <button
            type="button"
            onClick={addChoice}
            className="
              flex items-center gap-1 text-sm font-semibold
              text-red-600 transition-colors hover:text-red-700
            "
          >
            <PlusIcon className="h-4 w-4 stroke-[2.5px]" />
            Add Choice
          </button>
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
