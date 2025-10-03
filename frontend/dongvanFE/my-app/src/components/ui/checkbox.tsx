"use client"

export const Checkbox = ({ checked, onCheckedChange, id }: any) => (
  <input id={id} type="checkbox" checked={!!checked} onChange={(e) => onCheckedChange?.(e.target.checked)} />
)

export default Checkbox
