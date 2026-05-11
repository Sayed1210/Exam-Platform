import { Check } from "lucide-react";
import type { QuestionChoice } from "@/types/question";

export type ChoiceItemProps = {
  choice: QuestionChoice;
};

export default function ChoiceItem({ choice }: ChoiceItemProps) {
  return (
    <li
      className={[
        "flex items-start gap-2 text-sm",
        choice.isCorrect
          ? "text-emerald-600"
          : "text-gray-500",
      ].join(" ")}
    >
      <span
        className={[
          "mt-0.5 h-4 w-4 shrink-0 rounded-full border",
          choice.isCorrect
            ? "border-emerald-500 bg-emerald-500"
            : "border-gray-300 bg-white",
        ].join(" ")}
        aria-hidden="true"
      />

      <span className="min-w-0 flex-1">
        {choice.imageUrl ? (
          <div className="flex justify-center overflow-hidden rounded-md border border-gray-200 bg-gray-50  max-h-16">
            {/* eslint-disable-next-line @next/next/no-img-element */}
            <img
              src={choice.imageUrl}
              alt="Choice image"
              className="max-h-16 object-contain"
            />
          </div>
        ) : (
          <span className="break-words">
            {choice.text}
          </span>
        )}

        {choice.isCorrect ? (
          <Check size={14} className="ml-1 inline" />
        ) : null}
      </span>
    </li>
  );
}
