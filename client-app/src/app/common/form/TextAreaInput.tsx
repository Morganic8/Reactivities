import React from 'react';
import { FieldRenderProps } from 'react-final-form';
import { FormFieldProps, Label, Form } from 'semantic-ui-react';

interface IProps
  extends FieldRenderProps<string, HTMLElement>,
    FormFieldProps {}

const TextAreaInput: React.FC<IProps> = ({
  input,
  width,
  name,
  rows,
  placeholder,
  meta: { touched, error },
}) => {
  return (
    <Form.Field name={name} error={touched && !!error} width={width}>
      <textarea rows={rows} placeholder={placeholder} {...input} />
      {touched && error && (
        <Label basic color="red">
          {error}
        </Label>
      )}
    </Form.Field>
  );
};

export default TextAreaInput;
