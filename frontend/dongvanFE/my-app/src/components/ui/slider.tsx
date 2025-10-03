"use client"

export const Slider = ({ value, onValueChange, min = 0, max = 100, step = 1, className = "" }: any) => {
  // placeholder using two range inputs could be improved; for now support array value
  return (
    <input
      type="range"
      min={min}
      max={max}
      step={step}
      value={Array.isArray(value) ? value[0] : value}
      onChange={(e) => onValueChange && onValueChange([Number(e.target.value), Array.isArray(value) ? value[1] : Number(e.target.value)])}
      className={className}
    />
  )
}

export default Slider
