import React from 'react';
import { FieldRenderProps } from 'react-final-form';
import { FormFieldProps, Label, Form, Select } from 'semantic-ui-react';

interface IProps
  extends FieldRenderProps<string, HTMLElement>,
    FormFieldProps {}
const SelectInput: React.FC<IProps> = ({
  input,
  name,
  width,
  options,
  placeholder,
  meta: { touched, error },
}) => {
  return (
    <Form.Field name={name} error={touched && !!error} width={width}>
      <Select
        value={input.value}
        onChange={(e, data) => input.onChange(data.value)}
        placeholder={placeholder}
        options={options}
      />
      {touched && error && (
        <Label basic color="red">
          {error}
        </Label>
      )}
    </Form.Field>
  );
};

export default SelectInput;
