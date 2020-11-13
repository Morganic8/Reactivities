import React from 'react';
import { FieldRenderProps } from 'react-final-form';
import { FormFieldProps, Label, Form } from 'semantic-ui-react';
import { DateTimePicker } from 'react-widgets';

interface IProps extends FieldRenderProps<any, HTMLElement>, FormFieldProps {}

const DateInput: React.FC<IProps> = ({
  input,
  width,
  name,
  placeholder,
  date = false,
  time = false,
  meta: { touched, error },
  ...rest
}) => {
  return (
    <Form.Field name={name} error={touched && !!error} width={width}>
      <DateTimePicker
        placeholder={placeholder}
        value={input.value || null}
        onChange={input.onChange}
        onBlur={input.onBlur}
        onKeyDown={(e) => e.preventDefault()}
        {...rest}
        date={date}
        time={time}
      />
      {touched && error && (
        <Label basic color="red">
          {error}
        </Label>
      )}
    </Form.Field>
  );
};

export default DateInput;
