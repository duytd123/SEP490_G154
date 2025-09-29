"use client"

import { useState } from "react"

type CalendarProps = {
  mode?: "single" | "range"
  selected?: Date
  onSelect?: (d: Date | undefined) => void
  disabled?: (d: Date) => boolean
  initialFocus?: boolean
}

function startOfMonth(d: Date) {
  return new Date(d.getFullYear(), d.getMonth(), 1)
}

function endOfMonth(d: Date) {
  return new Date(d.getFullYear(), d.getMonth() + 1, 0)
}

function addMonths(d: Date, n: number) {
  return new Date(d.getFullYear(), d.getMonth() + n, 1)
}

function isSameDay(a?: Date, b?: Date) {
  if (!a || !b) return false
  return a.getFullYear() === b.getFullYear() && a.getMonth() === b.getMonth() && a.getDate() === b.getDate()
}

function formatMonthYear(d: Date) {
  return d.toLocaleString(undefined, { month: "long", year: "numeric" })
}

export const Calendar = ({ selected, onSelect, disabled }: CalendarProps) => {
  const today = new Date()
  const [current, setCurrent] = useState<Date>(startOfMonth(selected || today))

  const firstDayIndex = startOfMonth(current).getDay() // 0 (Sun) - 6
  const daysInMonth = endOfMonth(current).getDate()

  const days: (Date | null)[] = []
  for (let i = 0; i < firstDayIndex; i++) days.push(null)
  for (let d = 1; d <= daysInMonth; d++) days.push(new Date(current.getFullYear(), current.getMonth(), d))

  return (
    <div className="bg-white border rounded-lg shadow p-3 w-64 text-sm">
      <div className="flex items-center justify-between mb-2">
        <button
          type="button"
          onClick={() => setCurrent((c) => addMonths(c, -1))}
          className="px-2 py-1 rounded hover:bg-gray-100"
        >
          ‹
        </button>
        <div className="font-medium">{formatMonthYear(current)}</div>
        <button
          type="button"
          onClick={() => setCurrent((c) => addMonths(c, 1))}
          className="px-2 py-1 rounded hover:bg-gray-100"
        >
          ›
        </button>
      </div>

      <div className="grid grid-cols-7 text-center text-xs text-gray-500 mb-1">
        {['Su','Mo','Tu','We','Th','Fr','Sa'].map((wd) => (
          <div key={wd} className="py-1">{wd}</div>
        ))}
      </div>

      <div className="grid grid-cols-7 gap-1">
        {days.map((dt, idx) => {
          if (!dt) return <div key={"empty-" + idx} />
          const isToday = isSameDay(dt, today)
          const isSelected = isSameDay(dt, selected)
          const isDisabled = disabled ? disabled(dt) : false

          const base = "w-8 h-8 flex items-center justify-center rounded"
          const cls = isDisabled
            ? base + " text-gray-300"
            : isSelected
            ? base + " bg-blue-600 text-white"
            : isToday
            ? base + " ring-1 ring-blue-200"
            : base + " hover:bg-gray-100"

          return (
            <button
              key={dt.toISOString()}
              type="button"
              disabled={isDisabled}
              onClick={() => onSelect?.(new Date(dt.getFullYear(), dt.getMonth(), dt.getDate()))}
              className={cls}
            >
              {dt.getDate()}
            </button>
          )
        })}
      </div>
    </div>
  )
}

export default Calendar
