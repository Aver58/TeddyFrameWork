# -*- coding: utf-8 -*-
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: Test.proto

import sys
_b=sys.version_info[0]<3 and (lambda x:x) or (lambda x:x.encode('latin1'))
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from google.protobuf import reflection as _reflection
from google.protobuf import symbol_database as _symbol_database
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()




DESCRIPTOR = _descriptor.FileDescriptor(
  name='Test.proto',
  package='HiProtobuf',
  syntax='proto3',
  serialized_options=_b('\n\031com.HiProtobuf.HiProtobufB\016Test_classname\252\002\nHiProtobuf'),
  serialized_pb=_b('\n\nTest.proto\x12\nHiProtobuf\"K\n\x04Test\x12\n\n\x02id\x18\x01 \x01(\x05\x12\x0c\n\x04name\x18\x02 \x01(\t\x12\n\n\x02hp\x18\x03 \x01(\x05\x12\x0e\n\x06\x61ttack\x18\x04 \x01(\x05\x12\r\n\x05infos\x18\x05 \x03(\t\"{\n\nExcel_Test\x12.\n\x04\x44\x61ta\x18\x01 \x03(\x0b\x32 .HiProtobuf.Excel_Test.DataEntry\x1a=\n\tDataEntry\x12\x0b\n\x03key\x18\x01 \x01(\x05\x12\x1f\n\x05value\x18\x02 \x01(\x0b\x32\x10.HiProtobuf.Test:\x02\x38\x01\x42\x38\n\x19\x63om.HiProtobuf.HiProtobufB\x0eTest_classname\xaa\x02\nHiProtobufb\x06proto3')
)




_TEST = _descriptor.Descriptor(
  name='Test',
  full_name='HiProtobuf.Test',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='id', full_name='HiProtobuf.Test.id', index=0,
      number=1, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='name', full_name='HiProtobuf.Test.name', index=1,
      number=2, type=9, cpp_type=9, label=1,
      has_default_value=False, default_value=_b("").decode('utf-8'),
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='hp', full_name='HiProtobuf.Test.hp', index=2,
      number=3, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='attack', full_name='HiProtobuf.Test.attack', index=3,
      number=4, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='infos', full_name='HiProtobuf.Test.infos', index=4,
      number=5, type=9, cpp_type=9, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=26,
  serialized_end=101,
)


_EXCEL_TEST_DATAENTRY = _descriptor.Descriptor(
  name='DataEntry',
  full_name='HiProtobuf.Excel_Test.DataEntry',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='key', full_name='HiProtobuf.Excel_Test.DataEntry.key', index=0,
      number=1, type=5, cpp_type=1, label=1,
      has_default_value=False, default_value=0,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
    _descriptor.FieldDescriptor(
      name='value', full_name='HiProtobuf.Excel_Test.DataEntry.value', index=1,
      number=2, type=11, cpp_type=10, label=1,
      has_default_value=False, default_value=None,
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[],
  enum_types=[
  ],
  serialized_options=_b('8\001'),
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=165,
  serialized_end=226,
)

_EXCEL_TEST = _descriptor.Descriptor(
  name='Excel_Test',
  full_name='HiProtobuf.Excel_Test',
  filename=None,
  file=DESCRIPTOR,
  containing_type=None,
  fields=[
    _descriptor.FieldDescriptor(
      name='Data', full_name='HiProtobuf.Excel_Test.Data', index=0,
      number=1, type=11, cpp_type=10, label=3,
      has_default_value=False, default_value=[],
      message_type=None, enum_type=None, containing_type=None,
      is_extension=False, extension_scope=None,
      serialized_options=None, file=DESCRIPTOR),
  ],
  extensions=[
  ],
  nested_types=[_EXCEL_TEST_DATAENTRY, ],
  enum_types=[
  ],
  serialized_options=None,
  is_extendable=False,
  syntax='proto3',
  extension_ranges=[],
  oneofs=[
  ],
  serialized_start=103,
  serialized_end=226,
)

_EXCEL_TEST_DATAENTRY.fields_by_name['value'].message_type = _TEST
_EXCEL_TEST_DATAENTRY.containing_type = _EXCEL_TEST
_EXCEL_TEST.fields_by_name['Data'].message_type = _EXCEL_TEST_DATAENTRY
DESCRIPTOR.message_types_by_name['Test'] = _TEST
DESCRIPTOR.message_types_by_name['Excel_Test'] = _EXCEL_TEST
_sym_db.RegisterFileDescriptor(DESCRIPTOR)

Test = _reflection.GeneratedProtocolMessageType('Test', (_message.Message,), {
  'DESCRIPTOR' : _TEST,
  '__module__' : 'Test_pb2'
  # @@protoc_insertion_point(class_scope:HiProtobuf.Test)
  })
_sym_db.RegisterMessage(Test)

Excel_Test = _reflection.GeneratedProtocolMessageType('Excel_Test', (_message.Message,), {

  'DataEntry' : _reflection.GeneratedProtocolMessageType('DataEntry', (_message.Message,), {
    'DESCRIPTOR' : _EXCEL_TEST_DATAENTRY,
    '__module__' : 'Test_pb2'
    # @@protoc_insertion_point(class_scope:HiProtobuf.Excel_Test.DataEntry)
    })
  ,
  'DESCRIPTOR' : _EXCEL_TEST,
  '__module__' : 'Test_pb2'
  # @@protoc_insertion_point(class_scope:HiProtobuf.Excel_Test)
  })
_sym_db.RegisterMessage(Excel_Test)
_sym_db.RegisterMessage(Excel_Test.DataEntry)


DESCRIPTOR._options = None
_EXCEL_TEST_DATAENTRY._options = None
# @@protoc_insertion_point(module_scope)